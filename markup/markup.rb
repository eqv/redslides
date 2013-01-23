require 'pp'
require 'json'

class Node

  attr_accessor :parent, :children, :attr

  def cmd(arg)
    @rules.merge! arg
  end

  def rules; end #override to call to cmd to initialize the rules
  def blocked; []; end #override to give a set of names that are blocked by this rule (e.g. may be available in the parents scope but not in this scope)

  def make_node(name,*args, &block)
    if @rules.include? name
      rule = @rules[name]
      return rule.call(*args,&block)
    end
    return nil if blocked.include? name
    return @parent.make_node(name,*args, &block) if @parent
    return nil
  end

  def type
    self.class.to_s.downcase
  end

  def method_missing(name,*args, &block)
    node = make_node(name,*args, &block)
    if node && node.class.ancestors.include?(Node)
      node.parent = self
      add_child(node)
      return :generated
    elsif node == :generated
    else
      super
    end
  end

  def add_child(node)
    @children << node
  end

  def initialize(parent, *attr, &block)
    if attr.any?{|x| x.class != Hash }
      raise("invalide attr given #{attr.inspect}")
    end
    @parent = parent
    @attr = {}
    @children = []
    @rules = {} 
    self.rules()
    attr.each do |hash|
      @attr.merge! hash
    end
      instance_eval(&block) if block
  end

  def to_json_obj()
    attr = @attr.map do |key,val|
      if val.respond_to? :to_json_obj
        [key, val.json_output]
      else
        [key,val]
      end.flatten
    end
    return {
      :type => self.type,
      :attr => Hash[attr],
      :children => self.children.map{|c| c.to_json_obj}
    }
  end

  def merge(*attr)
    attr.flatten.inject({}) { |s,e| s.merge e}
  end

end

class Img < Node
  def initialize(parent, src,*attr,&blck)
    super(parent, *attr,&blck)
    @attr.merge!({src: src})
  end
end

class Col < Node
end

class Slide < Node
  def rules
    cmd img: -> url,*attr,&blck { Img.new(self, *attr, {url: url}, &blck) },
    columns: -> *attr,&blck { Columns.new(self, *attr,&blck) }
  end
end

class Columns < Node
  def rules
    cmd col: -> *attr,&blck { Col.new(self, *attr,&blck) },
    twocol: -> *attr, &blck {
      attr = merge(*attr)
      columns do 
        col( &attr[:left])
        col( &attr[:right])
      end}
  end
end

require './presentation.rb'
#require './slide.rb'
#require './simple.rb'
