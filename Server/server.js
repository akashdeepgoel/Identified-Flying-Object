// server.js

// BASE SETUP
// =============================================================================

// call the packages we need
var express    = require('express');        // call express
var app        = express();                 // define our app using express
var bodyParser = require('body-parser');

// configure app to use bodyParser()
// this will let us get the data from a POST
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());

var port = process.env.PORT || 8080;        // set our port


// ROUTES FOR OUR API
// =============================================================================
var router = express.Router();              // get an instance of the express Router

// Adding ar-drone files
var arDrone = require('ar-drone');

//Making a cliet for ARDrone
var client_arDrone = arDrone.createClient();


// on routes that end in /takeoff
// ----------------------------------------------------
router.route('/takeoff')
         //create a takeoff (accessed at GET http://localhost:8080/api/takeoff)
	        .get(function(req, res){
                 console.log("Takeoff");
                 res.json({message : "Aaye aaye! Captain."});
                 client_arDrone.takeoff();
       		 client_arDrone.stop();

		client_arDrone.up(0.3);
		client_arDrone.stop();
		client_arDrone.stop();
		client_arDrone.stop();

         });
// on routes that end in /stop
// ----------------------------------------------------
router.route('/stop')
         //create a takeoff (accessed at GET http://localhost:8080/api/stop)
	        .get(function(req, res){
                 console.log("Stop");
                 res.json({message : "Hovering"});
                 client_arDrone.stop();
         });

// on routes that end in /land
// ----------------------------------------------------
router.route('/land')
	//create a land (accessed at GET http://localhost:8080/api/land)
	.get(function(req, res){
		console.log("Land");
		res.json({message : "We are going down."});
		client_arDrone.stop();
		client_arDrone.land();
		
	});

// on routes that end in /forward
// ----------------------------------------------------
router.route('/forward')
	//create a forward (accessed at GET http://localhost:8080/api/forward)
	.get(function(req,res){
		console.log("Forward");
		res.json({message : "The number one thing to do is to move forward."});
		client_arDrone.front(0.2);
		function myFunction(){
			client_arDrone.stop();
			console.log("Safety Maintained");
		}

		});

// on routes that end in /back
// ----------------------------------------------------
router.route('/back')
	//create a forward (accessed at GET http://localhost:8080/api/back)
	.get(function(req,res){
		console.log("Back");
		res.json({message : "We are going back."});
		client_arDrone.back(0.2);
		function myFunction(){
			client_arDrone.stop();
			console.log("Safety Maintained");
		}

		});

// on routes that end in /up
// ----------------------------------------------------
router.route('/up')
	//create a forward (accessed at GET http://localhost:8080/api/up)
	.get(function(req,res){
		console.log("Up");
		res.json({message : "Stay High All The Time!"});
		client_arDrone.up(0.2);

		});

// on routes that end in /right
// ----------------------------------------------------
router.route('/right')
	//create a forward (accessed at GET http://localhost:8080/api/right)
	.get(function(req,res){
		console.log("Right");
		res.json({message : "We are going right"});
		client_arDrone.right(0.2);
		setTimeout(myFunction, 1000);
		function myFunction(){
			client_arDrone.stop();
			console.log("Safety Maintained");
		}

		});

// on routes that end in /left
// ----------------------------------------------------
router.route('/left')
	//create a forward (accessed at GET http://localhost:8080/api/left)
	.get(function(req,res){
		console.log("Left");
		res.json({message : "We are going left"});
		client_arDrone.left(0.2);
		setTimeout(myFunction, 1000);
		function myFunction(){
			client_arDrone.stop();
			console.log("Safety Maintained");
		}

		});

// on routes that end in /dance
// ----------------------------------------------------
router.route('/dance')
	//create a forward (accessed at GET http://localhost:8080/api/dance)
	.get(function(req,res){
		console.log("Dance");
		res.json({message : "Sunn raha hai na tu, Naach raha hun mein"});
		client_arDrone.animate('wave',2000);
		
		});

// on routes that end in /animateLEDs
// ----------------------------------------------------
router.route('/animateLEDs')
	//create a forward (accessed at GET http://localhost:8080/api/animateLEDs)
	.get(function(req,res){
		console.log("Animate LEDs");
		res.json({message : "Let the game begin!"});
		client_arDrone.animateLeds('blinkGreenRed',5,4);

		});

// test route to make sure everything is working (accessed at GET http://localhost:8080/api)
router.get('/', function(req, res) {
    res.json({ message: 'hooray! welcome to our api!' });
    console.log("Someone visited. Open the gates");
    });

// more routes for our API will happen here

// REGISTER OUR ROUTES -------------------------------
// all of our routes will be prefixed with /api
app.use('/api', router);

// START THE SERVER
// =============================================================================
app.listen(port);
console.log('Magic happens on port ' + port);
