#include "entity.h"
#include "helper.h"
#include "shaders.h"

#include <GL\glut.h>
#include <iostream>

void Entity::SetLight()
{
	int active = 0;
	for (auto i = 0u; i < lights.size(); i++) {
		glUniform1i(lights[i].on, world.lightOn[i]);
		if (world.lightOn[i])active++;
		glUniform3f(lights[i].pos, world.lightPos[i].x, world.lightPos[i].y, world.lightPos[i].z);
	}
	glUniform1i(lcount, active);
}

void Entity::InitLight(GLuint program)
{
	auto size = world.lightOn.size();
	std::cout << size << std::endl;
	for (auto i = 0u; i < size; i++)
	{
		lights.push_back(LightUnif{
			getUniformLocation(("lights[" + std::to_string(i) + "].on").c_str(), program),
			getUniformLocation(("lights[" + std::to_string(i) + "].position").c_str(), program)
			});
	}
	lcount = getUniformLocation("activeLights", program);
}

//	*****************************************  //
//	**           TriangleEntity            **  //
//	*****************************************  //

TriangleEntity::TriangleEntity(World& world, const Mesh& mesh) :
	Entity(world), mesh(mesh)
{
	InitShaders();
	InitMesh();
}

TriangleEntity::~TriangleEntity()
{
	FreeShaders();
	FreeMesh();
}

void TriangleEntity::InitShaders()
{
	const char* vsSource = Shaders::simpleVertex.c_str();
	const char* frSource = Shaders::simplePhong.c_str();

	GLuint vShader = glCreateShader(GL_VERTEX_SHADER);
	glShaderSource(vShader, 1, &vsSource, NULL);
	glCompileShader(vShader);
	std::cout << "vertex shader \n";
	shaderLog(vShader);

	GLuint fShader = glCreateShader(GL_FRAGMENT_SHADER);
	glShaderSource(fShader, 1, &frSource, NULL);
	glCompileShader(fShader);
	std::cout << "fragment shader \n";
	shaderLog(fShader);

	program = glCreateProgram();
	glAttachShader(program, vShader);
	glAttachShader(program, fShader);
	glLinkProgram(program);
	int link_ok;
	glGetProgramiv(program, GL_LINK_STATUS, &link_ok);
	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}

	attribVertex = getAttribLocation("coord", program);
	attribNormal = getAttribLocation("normal", program);
	unifColor = getUniformLocation("color", program);
	unifTransform = getUniformLocation("transform", program);
	unifCamera = getUniformLocation("camera", program);
	unifProjection = getUniformLocation("projection", program);
	InitLight(program);
	checkOpenGLerror();
}

void TriangleEntity::InitMesh()
{

	glGenBuffers(1, &vertexVbo);
	glBindBuffer(GL_ARRAY_BUFFER, vertexVbo);
	glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * mesh.points.size() * 3, mesh.points.data(), GL_STATIC_DRAW);
	glGenBuffers(1, &normalVbo);
	glBindBuffer(GL_ARRAY_BUFFER, normalVbo);
	glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * mesh.normals.size() * 3, mesh.normals.data(), GL_STATIC_DRAW);
	checkOpenGLerror();
}

void TriangleEntity::FreeShaders()
{
	glUseProgram(0);
	glDeleteProgram(program);
}

void TriangleEntity::FreeMesh()
{
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDeleteBuffers(1, &vertexVbo);
	glDeleteBuffers(1, &normalVbo);
}

void TriangleEntity::Draw(const Matrix& m)
{
	Matrix c = world.Camera();
	Matrix p = world.Projection();

	glUseProgram(program);

	glEnableVertexAttribArray(attribVertex);
	glBindBuffer(GL_ARRAY_BUFFER, vertexVbo);
	glVertexAttribPointer(attribVertex, 3, GL_FLOAT, GL_FALSE, 0, 0);
	glEnableVertexAttribArray(attribNormal);
	glBindBuffer(GL_ARRAY_BUFFER, normalVbo);
	glVertexAttribPointer(unifColor, 3, GL_FLOAT, GL_FALSE, 0, 0);
	GLfloat color[3] = { mesh.diffuse.r, mesh.diffuse.g, mesh.diffuse.b};
	glUniform3fv(unifColor, 1, color);
	glUniformMatrix4fv(unifTransform, 1, GL_TRUE, m.Data());
	glUniformMatrix4fv(unifCamera, 1, GL_TRUE, c.Data());
	glUniformMatrix4fv(unifProjection, 1, GL_TRUE, p.Data());
	SetLight();

	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDrawArrays(GL_TRIANGLES, 0, mesh.normals.size());
	glDisableVertexAttribArray(unifColor);
	glDisableVertexAttribArray(attribVertex);

	glUseProgram(0);
	checkOpenGLerror();
}


//	*****************************************  //
//	**           TexturedEntity            **  //
//	*****************************************  //

TexturedEntity::TexturedEntity(World& world, const Mesh& mesh) :
	Entity(world), mesh(mesh)
{
	InitShaders();
	InitMesh();
}

TexturedEntity::~TexturedEntity()
{
	FreeShaders();
	FreeMesh();
}

void TexturedEntity::InitShaders()
{
	const char* vsSource = Shaders::simpleVertex.c_str();
	const char* frSource = Shaders::simpleTexture.c_str();

	GLuint vShader = glCreateShader(GL_VERTEX_SHADER);
	glShaderSource(vShader, 1, &vsSource, NULL);
	glCompileShader(vShader);
	std::cout << "vertex shader \n";
	shaderLog(vShader);

	GLuint fShader = glCreateShader(GL_FRAGMENT_SHADER);
	glShaderSource(fShader, 1, &frSource, NULL);
	glCompileShader(fShader);
	std::cout << "fragment shader \n";
	shaderLog(fShader);

	program = glCreateProgram();
	glAttachShader(program, vShader);
	glAttachShader(program, fShader);
	glLinkProgram(program);
	int link_ok;
	glGetProgramiv(program, GL_LINK_STATUS, &link_ok);
	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}

	attribVertex = getAttribLocation("coord", program);
	attribVColor = getAttribLocation("normal", program);
	attribTexture = getAttribLocation("texture", program);
	unifTransform = getUniformLocation("transform", program);
	unifCamera = getUniformLocation("camera", program);
	unifProjection = getUniformLocation("projection", program);
	unifTexmap = getUniformLocation("texmap", program);
	InitLight(program);

	checkOpenGLerror();
}

void TexturedEntity::InitMesh()
{
	glGenBuffers(1, &vertexVbo);
	glBindBuffer(GL_ARRAY_BUFFER, vertexVbo);
	glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * mesh.points.size() * 3, mesh.points.data(), GL_STATIC_DRAW);
	glGenBuffers(1, &normalVbo);
	glBindBuffer(GL_ARRAY_BUFFER, normalVbo);
	glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * mesh.normals.size() * 3, mesh.normals.data(), GL_STATIC_DRAW);
	glGenBuffers(1, &textureVbo);
	glBindBuffer(GL_ARRAY_BUFFER, textureVbo);
	glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * mesh.textures.size() * 2, mesh.textures.data(), GL_STATIC_DRAW);
	checkOpenGLerror();
}

void TexturedEntity::FreeShaders()
{
	glUseProgram(0);
	glDeleteProgram(program);
}

void TexturedEntity::FreeMesh()
{
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDeleteBuffers(1, &vertexVbo);
	glDeleteBuffers(1, &normalVbo);
	glDeleteBuffers(1, &textureVbo);
}

void TexturedEntity::Draw(const Matrix& m)
{
	Matrix c = world.Camera();
	Matrix p = world.Projection();

	glUseProgram(program);

	glEnableVertexAttribArray(attribVertex);
	glBindBuffer(GL_ARRAY_BUFFER, vertexVbo);
	glVertexAttribPointer(attribVertex, 3, GL_FLOAT, GL_FALSE, 0, 0);

	glEnableVertexAttribArray(attribVColor);
	glBindBuffer(GL_ARRAY_BUFFER, normalVbo);
	glVertexAttribPointer(attribVColor, 3, GL_FLOAT, GL_FALSE, 0, 0);

	glEnableVertexAttribArray(attribTexture);
	glBindBuffer(GL_ARRAY_BUFFER, textureVbo);
	glVertexAttribPointer(attribTexture, 2, GL_FLOAT, GL_FALSE, 0, 0);

	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, mesh.diffuseMap);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);

	glUniform1i(unifTexmap, 0);

	glUniformMatrix4fv(unifTransform, 1, GL_TRUE, m.Data());
	glUniformMatrix4fv(unifCamera, 1, GL_TRUE, c.Data());
	glUniformMatrix4fv(unifProjection, 1, GL_TRUE, p.Data());
	SetLight();

	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDrawArrays(GL_TRIANGLES, 0, mesh.normals.size());
	glDisableVertexAttribArray(attribVColor);
	glDisableVertexAttribArray(attribVertex);

	glUseProgram(0);
	checkOpenGLerror();
}


//	*****************************************  //
//	**        ColorTexturedEntity          **  //
//	*****************************************  //

ColorTexturedEntity::ColorTexturedEntity(World& world, const Mesh& mesh) :
	Entity(world), mesh(mesh)
{
	InitShaders();
	InitMesh();
}

ColorTexturedEntity::~ColorTexturedEntity()
{
	FreeShaders();
	FreeMesh();
}

void ColorTexturedEntity::InitShaders()
{
	const char* vsSource = Shaders::colorTextureVertex.c_str();
	const char* frSource = Shaders::colorTexture.c_str();

	GLuint vShader = glCreateShader(GL_VERTEX_SHADER);
	glShaderSource(vShader, 1, &vsSource, NULL);
	glCompileShader(vShader);
	std::cout << "vertex shader \n";
	shaderLog(vShader);

	GLuint fShader = glCreateShader(GL_FRAGMENT_SHADER);
	glShaderSource(fShader, 1, &frSource, NULL);
	glCompileShader(fShader);
	std::cout << "fragment shader \n";
	shaderLog(fShader);

	program = glCreateProgram();
	glAttachShader(program, vShader);
	glAttachShader(program, fShader);
	glLinkProgram(program);
	int link_ok;
	glGetProgramiv(program, GL_LINK_STATUS, &link_ok);
	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}

	attribVertex = getAttribLocation("coord", program);
	attribVColor = getAttribLocation("vcolor", program);
	attribNormal = getAttribLocation("normal", program);
	attribTexture = getAttribLocation("texture", program);
	unifTransform = getUniformLocation("transform", program);
	unifCamera = getUniformLocation("camera", program);
	unifProjection = getUniformLocation("projection", program);
	unifTexmap = getUniformLocation("texmap", program);
	InitLight(program);

	checkOpenGLerror();
}


Color ColorTexturedEntity::VertexColor(Point p)
{
	srand(((int)(p.x * 100)) | ((int)(p.y * 10)) | ((int)(p.z * 1000)));

	Color colors[] = {
		{0.0, 1.0, 1.0},	{0.0, 0.0, 1.0},	{0.0, 1.0, 0.0},	{1.0, 1.0, 0.0},
		{1.0, 0.0, 1.0},	{1.0, 0.0, 0.0},	{1.0, 1.0, 1.0},	{0.0, 0.0, 0.0},
		{0.5, 1.0, 1.0},	{0.5, 0.0, 1.0},	{0.5, 1.0, 0.5},	{1.0, 1.0, 0.5},
		{1.0, 0.5, 1.0},	{1.0, 0.5, 0.0},	{1.0, 1.0, 1.0},	{0.0, 0.5, 0.0}
	};

	return colors[rand() % 16];
}

void ColorTexturedEntity::InitMesh()
{
	glGenBuffers(1, &vertexVbo);
	glBindBuffer(GL_ARRAY_BUFFER, vertexVbo);
	glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * mesh.points.size() * 3, mesh.points.data(), GL_STATIC_DRAW);
	glGenBuffers(1, &normalVbo);
	glBindBuffer(GL_ARRAY_BUFFER, normalVbo);
	glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * mesh.normals.size() * 3, mesh.normals.data(), GL_STATIC_DRAW);
	glGenBuffers(1, &textureVbo);
	glBindBuffer(GL_ARRAY_BUFFER, textureVbo);
	glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * mesh.textures.size() * 2, mesh.textures.data(), GL_STATIC_DRAW);

	std::vector<Color> colors(mesh.textures.size());
	for (int i = 0; i < colors.size(); ++i)
		colors[i] = VertexColor(mesh.points[i]);

	glGenBuffers(1, &colorVbo);
	glBindBuffer(GL_ARRAY_BUFFER, colorVbo);
	glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * colors.size() * 3, colors.data(), GL_STATIC_DRAW);
	checkOpenGLerror();
}

void ColorTexturedEntity::FreeShaders()
{
	glUseProgram(0);
	glDeleteProgram(program);
}

void ColorTexturedEntity::FreeMesh()
{
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDeleteBuffers(1, &vertexVbo);
	glDeleteBuffers(1, &normalVbo);
	glDeleteBuffers(1, &textureVbo);
}

void ColorTexturedEntity::Draw(const Matrix& m)
{
	Matrix c = world.Camera();
	Matrix p = world.Projection();

	glUseProgram(program);

	glEnableVertexAttribArray(attribVertex);
	glBindBuffer(GL_ARRAY_BUFFER, vertexVbo);
	glVertexAttribPointer(attribVertex, 3, GL_FLOAT, GL_FALSE, 0, 0);

	glEnableVertexAttribArray(attribNormal);
	glBindBuffer(GL_ARRAY_BUFFER, normalVbo);
	glVertexAttribPointer(attribNormal, 3, GL_FLOAT, GL_FALSE, 0, 0);

	glEnableVertexAttribArray(attribVColor);
	glBindBuffer(GL_ARRAY_BUFFER, colorVbo);
	glVertexAttribPointer(attribVColor, 3, GL_FLOAT, GL_FALSE, 0, 0);

	glEnableVertexAttribArray(attribTexture);
	glBindBuffer(GL_ARRAY_BUFFER, textureVbo);
	glVertexAttribPointer(attribTexture, 2, GL_FLOAT, GL_FALSE, 0, 0);

	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, mesh.diffuseMap);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);

	glUniform1i(unifTexmap, 0);

	glUniformMatrix4fv(unifTransform, 1, GL_TRUE, m.Data());
	glUniformMatrix4fv(unifCamera, 1, GL_TRUE, c.Data());
	glUniformMatrix4fv(unifProjection, 1, GL_TRUE, p.Data());
	SetLight();

	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDrawArrays(GL_TRIANGLES, 0, mesh.normals.size());
	glDisableVertexAttribArray(attribVColor);
	glDisableVertexAttribArray(attribVertex);

	glUseProgram(0);
	checkOpenGLerror();
}


//	*****************************************  //
//	**       TexturedTexturedEntity        **  //
//	*****************************************  //

TexturedTexturedEntity::TexturedTexturedEntity(World& world, const Mesh& mesh) :
	Entity(world), mesh(mesh)
{
	InitShaders();
	InitMesh();
}

TexturedTexturedEntity::~TexturedTexturedEntity()
{
	FreeShaders();
	FreeMesh();
}

void TexturedTexturedEntity::InitShaders()
{
	const char* vsSource = Shaders::simpleVertex.c_str();
	const char* frSource = Shaders::textureTexture.c_str();

	GLuint vShader = glCreateShader(GL_VERTEX_SHADER);
	glShaderSource(vShader, 1, &vsSource, NULL);
	glCompileShader(vShader);
	std::cout << "vertex shader \n";
	shaderLog(vShader);

	GLuint fShader = glCreateShader(GL_FRAGMENT_SHADER);
	glShaderSource(fShader, 1, &frSource, NULL);
	glCompileShader(fShader);
	std::cout << "fragment shader \n";
	shaderLog(fShader);

	program = glCreateProgram();
	glAttachShader(program, vShader);
	glAttachShader(program, fShader);
	glLinkProgram(program);
	int link_ok;
	glGetProgramiv(program, GL_LINK_STATUS, &link_ok);
	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}

	attribVertex = getAttribLocation("coord", program);
	attribVColor = getAttribLocation("normal", program);
	attribTexture = getAttribLocation("texture", program);
	unifTransform = getUniformLocation("transform", program);
	unifIntensity = getUniformLocation("intensity", program);
	unifCamera = getUniformLocation("camera", program);
	unifProjection = getUniformLocation("projection", program);
	unifTexmap = getUniformLocation("texmap", program);
	unifMixtex = getUniformLocation("mixtex", program);
	InitLight(program);

	checkOpenGLerror();
}

void TexturedTexturedEntity::InitMesh()
{
	glGenBuffers(1, &vertexVbo);
	glBindBuffer(GL_ARRAY_BUFFER, vertexVbo);
	glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * mesh.points.size() * 3, mesh.points.data(), GL_STATIC_DRAW);
	glGenBuffers(1, &normalVbo);
	glBindBuffer(GL_ARRAY_BUFFER, normalVbo);
	glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * mesh.normals.size() * 3, mesh.normals.data(), GL_STATIC_DRAW);
	glGenBuffers(1, &textureVbo);
	glBindBuffer(GL_ARRAY_BUFFER, textureVbo);
	glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * mesh.textures.size() * 2, mesh.textures.data(), GL_STATIC_DRAW);
	checkOpenGLerror();
}

void TexturedTexturedEntity::FreeShaders()
{
	glUseProgram(0);
	glDeleteProgram(program);
}

void TexturedTexturedEntity::FreeMesh()
{
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDeleteBuffers(1, &vertexVbo);
	glDeleteBuffers(1, &normalVbo);
	glDeleteBuffers(1, &textureVbo);
}

void TexturedTexturedEntity::Draw(const Matrix& m)
{
	Matrix c = world.Camera();
	Matrix p = world.Projection();

	glUseProgram(program);

	glEnableVertexAttribArray(attribVertex);
	glBindBuffer(GL_ARRAY_BUFFER, vertexVbo);
	glVertexAttribPointer(attribVertex, 3, GL_FLOAT, GL_FALSE, 0, 0);

	glEnableVertexAttribArray(attribVColor);
	glBindBuffer(GL_ARRAY_BUFFER, normalVbo);
	glVertexAttribPointer(attribVColor, 3, GL_FLOAT, GL_FALSE, 0, 0);

	glEnableVertexAttribArray(attribTexture);
	glBindBuffer(GL_ARRAY_BUFFER, textureVbo);
	glVertexAttribPointer(attribTexture, 2, GL_FLOAT, GL_FALSE, 0, 0);

	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, mesh.diffuseMap);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	glUniform1i(unifTexmap, 0);

	glActiveTexture(GL_TEXTURE1);
	glBindTexture(GL_TEXTURE_2D, mesh.ambientMap);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
	glUniform1i(unifMixtex, 1);

	glUniform1f(unifIntensity, intensity);

	glUniformMatrix4fv(unifTransform, 1, GL_TRUE, m.Data());
	glUniformMatrix4fv(unifCamera, 1, GL_TRUE, c.Data());
	glUniformMatrix4fv(unifProjection, 1, GL_TRUE, p.Data());
	
	SetLight();
	
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDrawArrays(GL_TRIANGLES, 0, mesh.normals.size());
	glDisableVertexAttribArray(attribVColor);
	glDisableVertexAttribArray(attribVertex);

	glUseProgram(0);
	checkOpenGLerror();
}
