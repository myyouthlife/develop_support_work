var express=require('express');
var app=express();
app.get('/',function(req,res){
	
	res.send('hello world')
})

var server=app.listen(8888,function(){
	
	var host=server.address().adress;
	var port=server.address().port;
	console.log("htt://%s:%s",host,port);
})