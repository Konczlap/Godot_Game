extends Control

@onready var main_buttons: VBoxContainer = $MainButtons
@onready var options: Panel = $Options
@onready var resolutions_option_button: OptionButton = $Options/VBoxContainer/OptionButton

func add_resolutions():
	for r in GUI.resolutions:
		var text = str(r.x) + "x" + str(r.y)
		resolutions_option_button.add_item(text)
		
func update_button_values():
	var window_size_string = str(get_window().size.x, get_window().size.y)
	var resolutions_index = GUI.resolutions.keys().find(window_size_string)
	resolutions_option_button.selected = resolutions_index
	
	

func _process(_delta: float) -> void:
	pass

func _ready() -> void:
	add_resolutions()
	main_buttons.visible = true
	options.visible = false

func _on_startgame_pressed() -> void:
	SaveManager.StartNewGame = true
	get_tree().change_scene_to_file("res://Sceny/Sandbox.tscn")

func _on_continue_pressed() -> void:
	if SaveManager.LoadSave():
		get_tree().change_scene_to_file("res://Sceny/Sandbox.tscn")
	else:
		print("❌ Brak zapisu, nie można kontynuować!")

func _on_settings_pressed() -> void:
	print("Settings pressed")
	main_buttons.visible = false
	options.visible = true


func _on_exit_pressed() -> void:
	get_tree().quit()


func _on_back_options_pressed() -> void:
	main_buttons.visible = true
	options.visible = false


func _on_option_button_item_selected(index: int) -> void:
	var key = resolutions_option_button.get_item_text(index)
	get_window().set_size(GUI.resolutions[index])
