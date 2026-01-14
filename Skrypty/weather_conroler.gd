extends StaticBody2D

var current_weather = "rain"


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	if current_weather == "none":
		$rain.visible = false
		$raincolor.visible = false
	if current_weather == "rain":
		$rain.visible = true
		$raincolor.visible = true
		

# Called every frame. 'delta' is the elapsed time since the previous frame.
