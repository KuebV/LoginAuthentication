# LoginAuthentication
## About
Login Authentication is a Library to assist with other personal projects. 

It works by checking Hostnames, IP's, Authentication Strings, and Identities sent by the client

### Storage Methods
- Local Storage 
  - For Local Storage, you use [JsonDB](https://github.com/KuebV/JsonDB)

- Cloud Storage with MongoDB Atlas

### Dependencies
```csharp
- Newtonsoft.Json
- JsonDB
- MongoDB.Bson
- MongoDB.Driver
- MongoDB.Driver.Core 
// (Just download it from Nuget)
```

### Data Format
```csharp
{
  _id : Shortened Hash
  HostName : Hostname of the Connecting Client
  IPAddress : IP of the Connecting Client
  AuthenticationString : Hashed SHA256
}
```

### Conflicting Issues & References
Due to JsonDB being a key part in how LoginAuthentication works and functions with Local Storage, it may cause some conflicting references
This is not an issue as long as you specify the library

# Starting Out

## Starting the Server (Local Storage)
```csharp
using LoginAuthentication;

static void Main(string args[])
{
  // Server variable may not be used again, its simply there to setup the constructor and class
  Server server = new Server(UseLocalStorage: true, MongoURL:null);
  
  // Start the Database, Database Name first, Collection name second
  LoginAuthentication.Database.StartDatabase("LocalAuthentication", "ExampleAuthentication");
}
```

## Starting the Server (MongoDB)
```csharp
using LoginAuthentication;

static void Main(string args[])
{
  // Server variable may not be used again, its simply there to setup the constructor and class
  Server server = new Server(UseLocalStorage: false, MongoURL:"mongodb+srv://placeholder:placeholder@cluster.lxrev.mongodb.net/user?retryWrites=true&w=majority");
  
  // Since MongoDB & JsonDB are so similar, we are able to have these both be the same thing
  LoginAuthentication.Database.StartDatabase("LocalAuthentication", "ExampleAuthentication");
}
```

# Handling Client Data
```csharp
using LoginAuthentication;

static void Main(string args[])
{
  Server server = new Server(UseLocalStorage: true, MongoURL:null);
  LoginAuthentication.Database.StartDatabase("LocalAuthentication", "ExampleAuthentication");
  
  var client = new ClientDatabase
  {
    _id : "3C2B70D35FB3CA",
    Hostname : "DESKTOP-SERVER",
    IPAddress : "127.0.0.1",
    AuthenticationString : "4151A307E4861255BE822F94B1D4579F3ACCC0C6D58F2C24E4E3C2711D54EBD4"
  };
  
  Database.CreateNewItem("3C2B70D35FB3CA", clientData: client);
}
```

## Connection Settings
```csharp
using LoginAuthentication;

static void Main(string args[])
{
  Server server = new Server(UseLocalStorage: true, MongoURL:null);
  LoginAuthentication.Database.StartDatabase("LocalAuthentication", "ExampleAuthentication");
  
  var settings = new ClientAuthenticationSettings
  {
    HostNameMatch = false,
    IPMatch = false
   };
   ClientData clientData = new ClientData(incoming client id, settings);
}
```

## Handle Login Data & Response
```csharp
using LoginAuthentication;

static void Main(string args[])
{
  Server server = new Server(UseLocalStorage: true, MongoURL:null);
  LoginAuthentication.Database.StartDatabase("LocalAuthentication", "ExampleAuthentication");
  
  var settings = new ClientAuthenticationSettings
  {
    HostNameMatch = false,
    IPMatch = false
   };
   ClientData clientData = new ClientData(0, settings);
   
   LoginResponse loginResponse = clientData.HandleLoginData("127.0.0.1", "DESKTOP-SERVER", "4151A307E4861255BE822F94B1D4579F3ACCC0C6D58F2C24E4E3C2711D54EBD4", "3C2B70D35FB3CA")
   // Types of Responses
   // Good
   // Failed
   // IPMatch_False
   // HostNameMatch_False
   // ServerError
   // Unknownx
}
```
