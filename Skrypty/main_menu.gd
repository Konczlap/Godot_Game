extends Control

@onready var main_buttons: VBoxContainer = $MainButtons
@onready var options: Panel = $Options

@onready var resolutions_option_button: OptionButton = \
	$Options/VBoxContainer/GraphicsSection/ResolutionOptionButton


func _ready() -> void:
	# Menu główne widoczne, opcje ukryte
	main_buttons.visible = true
	options.visible = false

	add_resolutions()
	update_button_values()


# =======================
# ROZDZIELCZOŚĆ
# =======================

func add_resolutions() -> void:
	if resolutions_option_button == null:
		push_error("❌ ResolutionOptionButton not found!")
		return

	resolutions_option_button.clear()

	for r in GUI.resolutions:
		var text := str(r.x) + "x" + str(r.y)
		resolutions_option_button.add_item(text)


func update_button_values() -> void:
	var current_size := get_window().size
	var current_text := str(current_size.x) + "x" + str(current_size.y)

	for i in range(resolutions_option_button.item_count):
		if resolutions_option_button.get_item_text(i) == current_text:
			resolutions_option_button.selected = i
			return


func _on_resolution_option_button_item_selected(index: int) -> void:
	if index < 0:
		return

	var text := resolutions_option_button.get_item_text(index)
	var parts := text.split("x")

	if parts.size() != 2:
		return

	var width := int(parts[0])
	var height := int(parts[1])

	get_window().set_size(Vector2i(width, height))


# =======================
# PRZYCISKI MENU
# =======================

func _on_startgame_pressed() -> void:
	SaveManager.StartNewGame = true
	get_tree().change_scene_to_file("res://Sceny/Sandbox.tscn")


func _on_continue_pressed() -> void:
	if SaveManager.LoadSave():
		get_tree().change_scene_to_file("res://Sceny/Sandbox.tscn")
	else:
		print("❌ Brak zapisu, nie można kontynuować!")


func _on_settings_pressed() -> void:
	main_buttons.visible = false
	options.visible = true


func _on_back_options_pressed() -> void:
	main_buttons.visible = true
	options.visible = false


func _on_exit_pressed() -> void:
	get_tree().quit()
	

func _on_option_button_item_selected(index: int) -> void:
	pass # Replace with function body.
