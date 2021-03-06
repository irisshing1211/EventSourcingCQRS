# EventSourcingCQRS
This project is using CQRS + Event sourcing with SQL as Event store and NoSQL as the Read store. Also include how to do a snapshot.

This project is using **.Net Core 3.1 MVC** and **MongoDB**.

## Project Example

An application that increase or decrease count of item.
The count item will be create by seeder when application start.

## Ref
I follow these website to work on this project:
- CQRS: 
    - [microservices.io](https://microservices.io/patterns/data/cqrs.html)
    - [CQRS by Example: Simple ASP.NET CORE Implementation](https://dzone.com/articles/cqrs-by-example-simple-aspnet-core-implementation) and 
    - [Microsoft CQRS pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- Event Sourcing:
    - [Microservices With CQRS and Event Sourcing](https://dzone.com/articles/microservices-with-cqrs-and-event-sourcing) 
- MongoDB
    - [Create a web API with ASP.NET Core and MongoDB](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-3.1&tabs=visual-studio)
## Design
1. When increase / decrease the count, then controller will pass a command object to command handler.
2. The command handler will first insert a event log into event store, then call update count event to update object state.
3. There will be a snapshot between every 10 events for an object
4. The read store always restore from event store when application start
5. Select a time on the View page to rollback the events.
## Steps

### Preparation
1. create a new .Net Core MVC project
2. create event store object and event store DbContext
3. create **CountItem** object and read store context
4. use EF to create the event store db
5. add seeder to add 2 add item event for testing if event store is empty.
6. config mongodb

Connection String are all in appsettings.json
```json
 "DbSettings": {
    "CollectionName": "QueryCollection",
    "NoSqlConnString": "your mongo db connection string",
    "DatabaseName": "QueryDb",
    "RelationalConnString": "your sql connection string"
  },
```
### Event Parser
And now, we need write a event parser to convert event data to object or convert object to event data.

I use json string to store the old value and new value and my parser is using **Newtonsoft.Json** format.

For update action, event sourcing always need to record the old and new value, so I store the full state of the object for old value and **KeyValuePair** for updated field.

The parser will need the following function:
1. convert object to json string
2. convert json string to object
3. get updated field of an object
4. update an object from a json string

### Restore Read store
So now we can use the Event parser to restore the read store, which is MongoDB, from event store
1. in the seed data class, add **SeedMongo** function.
2. then foreach event in the event store
3. use event parser to convert the new value string to object
4. then insert into read store
5. if event action is update, then get the object from read store first and updated it with event parser.

### Command and Command Handler
After complete the parser, we can start to write the command.
1. create **UpdateCountCommand** class, which contain original item and updated item
2. create **CommandHandler** class for working on the command
3. in **CommandHandler** add a function to work with update item count and take an **UpdateCountCommand** object as parameter
`public void UpdateCount(UpdateCountCommand cmd){}`
4. in *UpdateCount* function, first insert the command into the event store
5. then now we will call the event to update item count
### Events
1. create **UpdateCountEvent** class. 
2. add **Push** function.
3. in push function, update the object state
### Query
We have inserted event and updated object, it's time to query them to check the result
1. create **ItemQueryService** class
2. add **GetAll** function to query all item
3. add **GetById** function to get 1 item by id
4. remember add scoped in **StartUp **
`services.AddScoped<IQueryService<CountItem>, ItemQueryService>();`
### Controller and view
Now, we have complete Command, Query and Event part. Let's link up them with controller and view
1. in the controller, remember to implement `IQueryService<CountItem>` in the constructor and assign to a private variable
2.  then add a private variable `CommandHandler _cmdHandler` and new it in the constructor
3. to get the data from read store, call `_queries.GetAll()` in the index action
4. add a table in the **Index.cshtml** to show the item list.
5. Then add **+1** and **-1** button for each of the item 
6. add **UpdateCount** action which is post and will take **item id** and **value** from the view, which will trigger when **+1** or **-1** button click
7. in **UpdateCount** action, first use the query to get item by item id
8. then new an **UpdateCountCommand** object
9. then call **UpdateCount** in the command handler with passing the command

### Rollback
To rollback a object to specific time, we will need a rollback event and command
1. create rollback command with contain item id and target time
2. create rollback event with takes list of event log and apply old value for each event
3. in command handler, add function to handler rollback command
4. go to home conntroller, add rollback action
5. in rollbak action, call rollback in command handler
### Snapshot
An object snapshot can take by number of events so that reduce the process time
1. in command handler, create snapshot function with passing event log object
2. in the function, check if the item in the log have the multiple number of 10 events, if yes then insert a snapshot record in event store.
3. in update count function, call the snapshot function.
