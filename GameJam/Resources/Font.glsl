#type vertex
#version 330 core
layout (location = 0) in vec2 vPos;
layout (location = 1) in vec2 vUv;

uniform mat4 uProjection;

out vec2 fUv;

void main()
{
    //Multiplying our uniform with the vertex position, the multiplication order here does matter.
    gl_Position = uProjection * vec4(vPos, 0.0, 1);
    fUv = vUv;
}

#type fragment
#version 330 core
in vec2 fUv;

uniform sampler2D uTexture0;
uniform vec4 uColor;

out vec4 FragColor;

void main()
{
    FragColor = uColor * texture(uTexture0, fUv);
}