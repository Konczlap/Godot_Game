extends Control

func _ready() -> void:
	pass


func _process(_delta: float) -> void:
	pass


func _on_button_pressed() -> void:
	# Zmienia scenę na główną grę
	get_tree().change_scene_to_file("res://Sceny/Sandbox.tscn")


func _on_settings_pressed() -> void:
	pass


func _on_exit_pressed() -> void:
	get_tree().quit()
