import sys
import web
import json

from IRCResponse import IRCResponse, ResponseType
from IRCMessage import IRCMessage

from FunctionHandler import AutoLoadFunctions
from GlobalVars import functions

class MessageHandler:
    def POST(self, name=None):
        data = web.data()
        jsonData = json.loads(data)
        message = IRCMessage(jsonData)
        print "Message Received: " + message.MessageString
        
        responses = []
        
        for name, func in functions.iteritems():
            try:
                response = func.GetResponse(message)
                if response != None:
                    responses.append( response.__dict__ )
            except Exception:
                responses.append( IRCResponse(\
                ResponseType.Say, \
                "Python Execution Error in '" + name +  "': " + str(sys.exc_info()), \
                message.ReplyTo).__dict__ )
        
        return json.dumps(responses)

urls = ('/message', 'MessageHandler')

if __name__ == "__main__":
    AutoLoadFunctions()
    app = web.application(urls, globals(), True)
    app.run()