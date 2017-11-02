# MTAResourceStats
Get stats of your MTA resources.  

A small tool to get statistics for all of your Lua-files in a resource-dictionary.   


## What can that program do?  
It can get the statistics for all the .lua files in a directory.  

The stats include:  
- amount of lua-files    
- amount of other files  
- amount of functions  
- amount of lines  
- amount of characters  
- amount of comment-lines  
- amount of comment-characters  

Also you can choose between a fast execution or a slow, but prettier.  
The fast execution could block all threads and that way also the UI.  
If you got many large .lua files you should better take the fast execution,   
else I would suggest to take the slower iterate.  


## I want more features!  

Then just suggest it in the Issues-tab:  
[Issues](https://github.com/emre1702/MTAResourceStats/issues)
