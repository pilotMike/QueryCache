#Query Cache  
  
This project is intended to project a quick and thread safe caching of function calls by their arguments and return type.
Specifically, I created this class to cache stored procedure calls by their arguments and return type in EF.
The original version of this class received a function and the arguments used, but I added the ability to provide an expression 
so that the developer does not have to copy-paste the arguments into the cache.

#Features  
* Can extract arguments used in an expression, such as a call to a stored procedure
* Allows custom arguments by using the GetOrAddFunc call

## Usages
The most common usage is to do a simple cache using the arguments as the keys.  
  
      var cache = new QueryCache();
      var context = new MyDbContext();
      for (int i = 0; i < 10; i++)
      {
          var result = cache.GetOrAdd(() => context.MyStoredProcedure("arguments here"));
          // do something
          // EF gets called once
      }

To use keys other than the arguments, use the GetOrAddFunc method  
  
      // in loop
      var result = cache.GetOrAddFunc(() => context.MyStoredProcedure("arguments here"))
                        .UsingKeys("custom arguments");
      
      // legacy sql calls can be used by providing the stored procedure name and arguments
      // for the stored procedure.
      DataTable table = cache.GetOrAddFunc(() => SqlHelper.ExecuteStoredProcedure("MyStoredProc", "my arguments"))
                             .UsingKeys("MyStoredProcedure", "my arguments");

#Considerations
This cache uses a new instance of MemoryCache rather than MemoryCache.Default.
While MemoryCache.Default is recommended, my purposes only required temporary caching for the lifetime of a request. 
The MemoryCache instance will last for the lifetime of the QueryCache object, so it can be managed in code or via
and IoC.
