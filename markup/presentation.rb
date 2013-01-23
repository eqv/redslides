
class Presentation < Node
  def rules
    cmd slide: -> title,*args,&blck { Slide.new(self,*args, {title: title}, &blck) }
  end

  def self.make(&block)
    return self.new(nil,&block)
  end
end
