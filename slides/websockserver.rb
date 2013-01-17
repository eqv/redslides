require 'websocket-eventmachine-server'
require 'set'
require 'securerandom'
require 'json'

$clients = Hash.new(Set.new)
EM.run do

  WebSocket::EventMachine::Server.start(:host => "127.0.0.1", :port => 8080) do |ws|

    ws.onopen do
      puts "Client connected"
    end

    session = nil
    master = false

    ws.onmessage do |msg, type|
      if !session
        msg = JSON.parse(msg)
        puts "preauth client msg #{msg.inspect}"
        if msg["type"]=="session"
          master_id = msg["master"]
          if master_id == "new"
            session = SecureRandom.hex
            puts "created new session with id #{session}"
            master = true
            $clients[session] = Set.new
            ws.send({"type" => "session", "id" => session}.to_json)
          else
            session = master_id
            if $clients.include? session
              puts "client joined session #{session}"
              $clients[session].add ws
            else
              puts "client joined with invalid session #{session} <- kicked"
              ws.close()
            end
          end
        end
      else
        puts "Received message: #{msg}"
        if master
          $clients[session].each do |client|
            client.send msg, :type => type unless ws == client
          end
        end
      end
    end

    ws.onclose do
      if master
        puts "Session #{session} terminated"
        $clients.delete session
      else
        puts "Client disconnected from session #{session}"
        $clients[session].delete ws
      end
    end
  end

end
