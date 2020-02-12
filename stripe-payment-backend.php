<?php
require_once('/home/sugges9/public_html/stripe-php/init.php');
// Set your secret key: remember to change this to your live secret key in production
// See your keys here: https://dashboard.stripe.com/account/apikeys
\Stripe\Stripe::setApiKey("sk_test_VExwCImpyMnsCRNX8fCJ2yth");

// Token is created using Checkout or Elements!
// Get the payment token ID submitted by the form:
$token = $_POST['stripeToken'];
$email = $_POST['stripeEmail'];
$amount = $_GET['amount'];
$tourneyID = $_GET['tourneyID'];
$participants = $_GET['participants'];
$newParticipant = $_GET['newParticipant'];
$PEmail = $_GET['PEmail'];

$charge = \Stripe\Charge::create([
    'amount' => $amount,
    'currency' => 'usd',
    'description' => 'Ultimate eSports Tournament Registration.',
    'source' => $token,
]);
?>
<!DOCTYPE html>
<html>
<head>
  <title>Payment Done</title>
  <script src="https://www.gstatic.com/firebasejs/5.0.1/firebase.js"></script>



<script>
  //document.getElementById("one").innerHTML = "Script works";
  // Initialize Firebase
  
  var config = {
    apiKey: "apikey8",
    authDomain: "fir-test-1bdb3.firebaseapp.com",
    databaseURL: "https://fir-test-1bdb3.firebaseio.com",
    projectId: "fir-test-1bdb3",
    storageBucket: "fir-test-1bdb3.appspot.com",
    messagingSenderId: "id"
  };
  firebase.initializeApp(config);
  var database = firebase.database();

  
  
</script>
<script src="https://www.gstatic.com/firebasejs/5.0.1/firebase-app.js"></script>
<script src="https://www.gstatic.com/firebasejs/5.0.1/firebase-database.js"></script>


</head>
<body>
    <h1>Payment is Done!</h1>
    <h2 id="one">Thank you <?php echo $email ?></h2>
    <h2>Thank you <?php echo $tourneyID ?></h2>
    <h2>Thank you <?php echo $PEmail ?></h2>
    <h2>Thank you <?php echo $participants ?></h2>
    <h2>Thank you <?php echo $newParticipant ?></h2>
    <h2 id="answer"></h2>
    
    
    <script type="text/javascript">
    
    var id = '<?php echo $tourneyID; ?>';
    var pemail = '<?php echo $PEmail; ?>';
    var participants = '<?php echo $participants; ?>';
    var newParticipant = '<?php echo $newParticipant; ?>';
    
    if (newParticipant === "true") {
        database.ref('tournaments/' +id+ '/Participants').set(pemail);
    }
    else {
        participants += ',' + pemail;
        database.ref('tournaments/' +id+ '/Participants').set(participants);
    }
    
  
  
  </script>
  <script>document.getElementById("one").innerHTML = id;</script>
</body>
</html>