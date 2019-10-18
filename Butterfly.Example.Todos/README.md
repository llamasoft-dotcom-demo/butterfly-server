# Get the Code

```
git clone https://github.com/firesharkstudios/butterfly-server
```

# Run the Server

To run in *Visual Studio*...
- Open *Butterfly.Server.sln*
- Run *Butterfly.Example.Todos*.

To run in a terminal or command prompt...
```
cd butterfly-server\Butterfly.Example.Todos
dotnet run
```

You can see the server code that runs at [Program.cs](https://github.com/firesharkstudios/butterfly-server/blob/master/Butterfly.Example.Todos/Program.cs).

## Run the Client

This assumes you have the [demo server](#run-the-server) running.

```
cd butterfly-server\Butterfly.Example.Todos\www
npm install
npm run serve
```

Now, open as many browser instances to http://localhost:8080/ as you wish to confirm the todo list stays synchronized across all connected clients.

**Note:** The server process is listening for API requests on port 8000 and the node dev server is listening on port 8080 and proxying API requests to port 8000.

# Understanding The Server

Here is all server code for our todo list manager...

```csharp
using System;

using Butterfly.Core.Util;

using Dict = System.Collections.Generic.Dictionary<string, object>;

namespace Butterfly.Example.Todos {
    class Program {
        static void Main(string[] args) {
            using (var embedIOContext = new Butterfly.EmbedIO.EmbedIOContext("http://+:8000/")) {
                // Create a MemoryDatabase (no persistence, limited features)
                var database = new Butterfly.Core.Database.Memory.MemoryDatabase();
                database.CreateFromText(@"CREATE TABLE todo (
	                id VARCHAR(50) NOT NULL,
	                name VARCHAR(40) NOT NULL,
	                PRIMARY KEY(id)
                );");
                database.SetDefaultValue("id", tableName => $"{tableName.Abbreviate()}_{Guid.NewGuid().ToString()}");

                // Listen for API requests
                embedIOContext.WebApi.OnPost("/api/todo/insert", async (req, res) => {
                    var todo = await req.ParseAsJsonAsync<Dict>();
                    await database.InsertAndCommitAsync<string>("todo", todo);
                });
                embedIOContext.WebApi.OnPost("/api/todo/delete", async (req, res) => {
                    var id = await req.ParseAsJsonAsync<string>();
                    await database.DeleteAndCommitAsync("todo", id);
                });

                // Listen for subscribe requests...
                // - The handler must return an IDisposable object (gets disposed when the channel is unsubscribed)
                // - The handler can push data to the client by calling channel.Queue()
                embedIOContext.SubscriptionApi.OnSubscribe("todos", (vars, channel) => {
                    return database.CreateAndStartDynamicViewAsync("SELECT * FROM todo", dataEventTransaction => channel.Queue(dataEventTransaction));
                });

                embedIOContext.Start();

                Console.ReadLine();
            }
        }
    }
}
```

The above C# code...
- Creates a Memory [database](#accessing-a-database) with a single *todo* table
- Defines a [Web API](#creating-a-web-api) to insert and delete *todo* records
- Defines a [Subscription API](#creating-a-subscription-api) to subscribe to a *todos* subscription

Clients are expected to...
- Use the subscription API to subscribe to the *todos* subscription to get a list of all initial *todo* records and any changes to the *todo* records
- Use the defined web API to insert and delete *todo* records

See [Program.cs](https://github.com/firesharkstudios/butterfly-server/tree/master/Butterfly.Example.Todos/Program.cs) for the working server code.

# Understanding The Client

Now, let's see how a client might interact with this server using the [Butterfly Client](#butterfly-client) javascript library.

First, the client should use *WebSocketChannelClient* to maintain an open WebSocket to the server...

```js
let channelClient = new WebSocketChannelClient({
    url: `ws://${window.location.host}/ws`
});
channelClient.connect();
```

Next, the client will want to subscribe to a channel to receive data...

```js
let todosList = [];
channelClient.subscribe({
    channel: 'todos',
    handler: new ArrayDataEventHandler({
        arrayMapping: {
            todo: todosList
        }
    })
});
```

This subscription will cause the local *todosList* array to be synchronized with the *todo* records on the server.

Next, let's invoke a method on our API to add a new *todo* record (use whatever client HTTP library you wish)...

```js
$.ajax('/api/todo/insert', {
  method: 'POST',
  data: JSON.stringify({
    name: 'My First To-Do',
  }),
});
```

After the above code runs, the server will have a new *todo* record and a new *todo* record will automagically be sychronized from the server to the client's local *todosList* array.

See [Butterfly.Example.Todos](https://github.com/firesharkstudios/butterfly-server/tree/master/Butterfly.Example.Todos/vue) for a full working client based on [Vuetify](https://vuetifyjs.com) and [Vue.js](https://vuejs.org/).