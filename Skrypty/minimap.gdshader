shader_type canvas_item;

void fragment() {
    vec2 center = vec2(0.5, 0.5);
    float dist = distance(UV, center);
    
    // Okrągła maska
    if (dist > 0.5) {
        COLOR.a = 0.0; // Przezroczyste poza kołem
    } else {
        COLOR = texture(TEXTURE, UV);
        
        // Opcjonalnie: ramka
        if (dist > 0.48) {
            COLOR = vec4(0.0, 0.0, 0.0, 1.0); // Biała ramka
        }
    }
}