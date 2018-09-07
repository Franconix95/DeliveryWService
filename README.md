# DeliveryWService

* URL test:
  http://193.126.247.98:4443/
  
* Added simple interface on API for manage and retrive best deliveries

* Added test data 
  * Points 
      [A,B,C,D,E,F,G,H,I]
  * Routes 
      A -> C : COST:20 | TIME: 1
      A -> E : COST:5 | TIME: 30
      A -> H : COST:10 | TIME: 1
      C -> B : COST:12 | TIME: 1
      D -> F : COST:50 | TIME: 4
      E -> D : COST:5 | TIME: 3
      F -> G : COST:50 | TIME: 40
      F -> I : COST:50 | TIME: 45
      G -> B : COST:73 | TIME: 64
      H -> E : COST:1 | TIME: 10
      H -> E : COST:1 | TIME: 10
      I -> B : COST:5 | TIME: 65               

* Best routing algorithm used:
    dijkstra

* Database:
  * MONGO DB
  * https://mlab.com/welcome/
  * mongodb://<dbuser>:<dbpassword>@ds243812.mlab.com:43812/deliverymanagerdb
