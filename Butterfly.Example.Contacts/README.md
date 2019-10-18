# Contact Manager Example

> A simple contact manager app using Vue.js, butterfly-client, and Butterfly.Server

The example shows...

- A Vue.js client interacting with a *Butterfly.Server* RESTlike API to add, update, and delete contacts
- A Vue.js client using the *butterfly-client* vanilla javascript library to receive updates when any data changes

# Get the Code

```
git clone https://github.com/firesharkstudios/butterfly-server
```

# Run the Server

```
cd butterfly-server\Butterfly.Example.Contacts
dotnet run
```

You can see the server code that runs at [Program.cs](https://github.com/firesharkstudios/butterfly-server/blob/master/Butterfly.Example.Contacts/Program.cs).

# Run the Client

```
cd butterfly-server\Butterfly.Example.Contacts\www
npm install
npm run serve
```

Open as many browser instances to http://localhost:8080/ as you wish to confirm the todo list stays synchronized across all connected clients.