<form action="getEvenNumbers.php" method="post">
    Enter a number: <input type="number" name="n">
    <input type="submit" value="Submit">
</form>

<?php
	if(isset($_REQUEST['n'])) {
		$number = $_REQUEST['n'];
		for ($i = 0; $i <= $number; $i += 2) {
			echo $i." ";
		}
	}
?>