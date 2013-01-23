class Simple < Node

  def type
    @type
  end

  def initialize(type,*attr,&block)
    @type = type
    default_init(attr,{})
  end
end

class Columns < Node
  def initialize(*attr)
    default_init(attr,{})
  end

  def col(*attr, &block)
    @attr[:columns] ||=[] # unless @attr.include? :columns
    @attr[:columns]<<div(&block)
  end
end

class Import < Node
  def initialize(path, *attr)
    default_init(attr, src: path)
  end
end

class Img < Node
  def initialize(path, *attr)
    default_init(attr, src: path)
  end
end

