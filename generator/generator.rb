require 'builder'
require 'rexml/document'

include REXML

class Generator

  def initialize()
    @rules = {}
    @defaults = {}
  end

  def default(type, *args)
    unless args.length == 1 && args.first.class == Hash
      raise "invalid default valuse #{args.inspect} a single hash was expected" 
    end
    @defaults[type.to_s]=args.first 
  end

  def rule(type,&block)
    @rules[type.to_s] = block
  end

  def gen(b,node)
    type = node.name
    attr = node.attributes
    children = node.elements.to_a

    r = @rules[ type ]
    if r
      attr = @defaults[type].merge attr if @defaults.include? type
      return r.call(b, attr, children)
    else
      raise("unknown rule for #{type.inspect}")
    end
  end

  def simple(b,tag,attr,children)
    b.send(tag,attr) do |bn|
      children.each do |ch|
        self.gen(bn,ch)
      end if children
    end
  end

  def simple_rule(*name)
    name.each do |n|
      n = n.to_s
      rule(n){ |b,attr,ch| simple(b,n,attr,ch) }
    end
  end

  def run(filename)
    xmlfile = File.new("test.pml")
    xmldoc = Document.new(xmlfile)
    root = xmldoc.root

    f = STDOUT
    #f = File.open("test.html")
    b = Builder::XmlMarkup.new(:target=>f, :indent=>2)
    gen(b, root)
  end

  def prefix(builder)
  end

  def postfix(builder)
    builder.comment!("Generated by redslides 0.1")
  end

end

g = Generator.new()
g.simple_rule(:div,:html,:body, :script, :link, :a, :h1, :h2, :h3, :img)

g.rule(:presentation) do |b,attr,children|
    g.prefix(b)
      children.each do |ch|
        unless ["slide","html"].include? ch.name
          raise "invalid child #{ch.name} in presentation" 
        end
        g.gen(b,ch)
      end
    g.postfix(b)
end

g.default(:slide, class: "slide") #Set.new(["slide"]) )
g.rule(:slide) do |b,attr,children|
    b.div(attr) do |b|
      children.each do |ch|
        g.gen(b,ch)
      end
    end
end

g.run("unused")
