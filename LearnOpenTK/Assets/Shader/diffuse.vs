// http://shader-playground.timjones.io/. 쉐이더 오류 체크

#version 430 core
 
// 버텍스당 입력
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec4 color;
layout (location = 3) in vec2 texcoord;

layout (location = 12) uniform mat4 mvp_matrix;
 
// 프래그먼트 쉐이더로의 출력
out VS_OUT
{
  vec2 texcoord;
  vec4 col;
} vs_out;
 
// 버텍스 쉐이더의 시작점
void main(void)
{
  vs_out.texcoord = texcoord;
  vs_out.col = color;

  gl_Position = mvp_matrix * vec4(position, 1);
}


