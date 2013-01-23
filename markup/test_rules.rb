require 'pp'
require './markup.rb'
p =Presentation.make do
  slide "foo" do
    columns do
      twocol left: -> parent do
        img "left"
      end, right: -> parent do
        img "right"
      end
    end
  end
end
pp p.to_json_obj
