<?php
class Book {
    private $name;
    private $price;
    
    function __construct($n,$p) {
        $this->$name = $n;
        $this->$price = $p;
    }
    
    public function __get($property) {
        if (property_exists($this, $property)) {
            return $this->$property;
        }
    }
    
    public static function GetDefaultBooks() {
        return array(new Book('Cat in the Hat',5), new Book('A Wrinkle in Time',10), new Book('The Giving Tree',15), new Book('Goodnight Moon', 20), new Book('Where the Wild Things Are',25));
    }
}
?>