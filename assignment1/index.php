<?php	
	class Player{
		private $name;
		private $team;
		private $ppg;
		private $gp;
		private $fgp;
		private $treyptm; //3ptm
		private $reb;
		private $ast;
		private $stl;
		private $blk;
		private $TO;
		
		//parameters: name, points per game, games played, 3points made, 
		// 		      rebounds, assists, steals, blocks, turnovers
		function __construct($n,$t,$p,$g,$f,$trey,$r,$a,$s,$b,$to) {
			$this->name = $n;
			$this->team = $t;
			$this->ppg = $p;
			$this->gp = $g;
			$this->fgp = $f;
			$this->treyptm = $trey;
			$this->reb = $r;
			$this->ast = $a;
			$this->stl = $s;
			$this->blk = $b;
			$this->TO = $to;
		}
		
        public function __get($property) {
            if (property_exists($this, $property)) {
                return $this->$property;
            }
        }
	}
	
	if(isset($_REQUEST['name'])) {
		$GLOBALS['players'] = array(); //global array to store query result set
		$inputName = $_REQUEST['name'];
		try {
			//connect to the database
			$conn = new PDO('mysql:host=nbastats.c7nrhyngsqie.us-west-2.rds.amazonaws.com:3306;dbname=info344assignment1','info344user','<password>');
			$conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
			
			//quote user input to prevent SQL injection
			$firstName = $conn->quote($inputName.'%');
			$lastName = $conn->quote('%'.$inputName);
			$fullName = $conn->quote($inputName);
			
			//query database and store result set
			foreach($conn->query("SELECT * FROM NBA_Stats_2015_2016 WHERE name LIKE $firstName OR name LIKE $lastName OR levenshtein($fullName,name) BETWEEN 0 AND 2") as $row) {
				//adds a new player to the array for each result row
				$GLOBALS['players'][] = new Player($row['name'],$row['team'],$row['points_per_game'],$row['games_played'],$row['FG_percentage'],$row['3PT_made'],$row['total_rebounds'],$row['assists'],$row['steals'],$row['blocks'],$row['turnovers']);
			}
		} catch (PDOException $e) {
			echo 'Error!: ' . $e->getMessage() . '<\br>';
			die();
		}
	}
	
	//compares based on games played.
	function compare($a, $b) {
		if ($a == $b) {
			return 0;
		}
		return ($a->gp > $b->gp) ? -1 : 1;
	}
	
	//displays the resulting array of players
	function displayPlayer() {
		if(isset($_REQUEST['name'])) {
			if(sizeof($GLOBALS['players']) == 0) {
				//outputs message when no players found with given name
				echo '<div class="statsBox"><h4>We\'re sorry, we could not find any players in our system with the name: "'.$_REQUEST['name'].'"</h4></div>';
			} else {
				usort($GLOBALS['players'], "compare"); //sorts array of players based on games played (most to least)
				foreach ($GLOBALS['players'] as $player) {
					
					//builds the image API request
					$parts = explode(' ', $player->name);
					$lastName = array_pop($parts);
					$firstName = implode(' ', $parts);
					$imgURL = "https://nba-players.herokuapp.com/players/".$lastName."/".$firstName;
					
					//checks if the nba headshot API contains the player's headshot, if not then a default image is used
					if (!is_array(@getimagesize($imgURL))) {
						$imgURL = "/img/defaultplayer.png";
					}
					
					//outputs the HTML
					echo 
					'<div class="statsBox">
							<img class="headshot" src="'.$imgURL.'" alt="'.$player->name.' headshot">
							<div class="heading">
								<h2>'.$player->name.'</h2>
								<h4>Team: <i>'.$player->team.'</i></h4>
							</div>
							<table class="statsTable">
								<tr>
									<th>Games Played</th>
									<th>PPG</th>
									<th>FG%</th>
									<th>3PTM</th>
									<th>Reb</th>
									<th>Ast</th>
									<th>Stl</th>
									<th>Blk</th>
									<th>TO</th>
								</tr>
								<tr>
									<td>'.$player->gp.'</td>
									<td>'.$player->ppg.'</td>
									<td>'.$player->fgp.'</td>
									<td>'.$player->treyptm.'</td>
									<td>'.$player->reb.'</td>
									<td>'.$player->ast.'</td>
									<td>'.$player->stl.'</td>
									<td>'.$player->blk.'</td>
									<td>'.$player->TO.'</td>
								</tr>
							</table>
					</div>';
				}
			}
		}
	}
?>
<html>
	<head>
	    <meta charset="UTF-8"/>
	    <meta name="viewport" content="width=device-width, initial-scale=1">
	
	    <title>NBA Stats 2015-2016 Season</title>
	
	  	<!-- bootstrap -->
  		<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css"/>
  		
  		<!-- mini logo on browser tab -->
	    <link rel="shortcut icon" href="img/nbalogo.png"/>
	    <!-- CSS stylesheet -->
	    <link rel="stylesheet" href="css/main.css"/>
	</head>
	<body>
		<!-- input/search bar, button and results area -->
		<div class="form-group" id="inputForm">
			<form action="index.php" method="post"> <!-- used post to clean up the URL after an input, get/post are both REQUEST -->
		    	<input type="text" id="name" name="name" class="form-control" placeholder="Type NBA Player Name"><br>
		    	<input type="submit" class="btn btn-primary" value="Search Stats">
			</form>
			
			<!-- displays the results -->
			<?php displayPlayer(); ?>
		</div>
		
		<footer>created by: Derek Han</footer>
	</body>
</html>

