<?php 
	if(isset($_REQUEST['callback']) && isset($_REQUEST['name'])) {
		$GLOBALS['players'] = array(); //global array to store query result set
		$inputName = $_REQUEST['name'];
		$callback = $_REQUEST['callback'];
		try {
			//connect to the database
			$conn = new PDO('mysql:host=nbastats.c7nrhyngsqie.us-west-2.rds.amazonaws.com:3306;dbname=info344assignment1','info344user','<password>');
			$conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
			
			$query = 'SELECT * FROM NBA_Stats_2015_2016 Where name = :name';
			$stmt = $conn->prepare($query);
			$stmt->bindParam(':name', $inputName);
			$stmt->execute();	
				
			
			$row = $stmt->fetch(PDO::FETCH_ASSOC);
			echo $callback."(".json_encode($row).")";
		} catch (PDOException $e) {
			echo 'Error!: ' . $e->getMessage();
			die();
		}
	}
?>

