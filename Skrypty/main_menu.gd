extends Control

func _ready() -> void:
	pass


func _process(_delta: float) -> void:
	pass


func _on_startgame_pressed() -> void:
	SaveManager.StartNewGame = true
	get_tree().change_scene_to_file("res://Sceny/Sandbox.tscn")

func _on_continue_pressed() -> void:
	if SaveManager.LoadSave():
		get_tree().change_scene_to_file("res://Sceny/Sandbox.tscn")
	else:
		print("❌ Brak zapisu, nie można kontynuować!")

func _on_settings_pressed() -> void:
	pass


func _on_exit_pressed() -> void:
	get_tree().quit()
