<html>
<head>
<title>Online PHP Script Execution</title>
</head>
<body>
<?php
   echo "<h1>Hello, PHP!</h1>";
   $myBool = TRUE;
   $myInt = 10;
   $myWord = "Hello World";
   $myArray = array("ice cream", "steak", "apples");
   echo $myArray[0];
   $myKeyValuePair = array("Dad" => "Joe", "Mom" => "Amy", "Bro" => "Jason");
   echo $myKeyValuePair["Dad"];
   
   echo "Array values:\n";
   foreach ($myArray as $value) {
       echo "$value\n";
   }
   
   echo "KeyValuePair values:\n";
   while (list($key, $val) = each($myKeyValuePair)) {
       echo "$key => $val\n";
   }   
   
   $temp = "10" + 1;
   echo "$temp\n";
   $temp = array(1,2,3);
   echo "$temp[1]\n";
   
   if (1 == "1") {
       echo "One\n";
   }
   
   if (1 === "1") {
       echo "Two\n";
   }
   
   echo 'hello' . " world\n";
?>
</body>
</html>
