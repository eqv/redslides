
class Slide < Node
  def initialize(title,*attr)
    default_init(attr, title: title)
  end

  def img(path,*attr)
    return add_child Img.new(path,*attr)
  end

  def columns(*attr, &block)
    return add_child Columns.new(*attr), &block
  end

  def simple(type,*attr, &block)
    add_child Simple.new(type, *attr) , &block
  end

  def div(*attr, &block)
    simple("div",*attr, &block)
  end

  def svg(path,*attr)
    return add_child Import.new(path,*attr)
  end
  alias :import :svg

  def enum(path,*attr, &block)
    return add_child Enum.new(path,*attr), &block
  end
end
