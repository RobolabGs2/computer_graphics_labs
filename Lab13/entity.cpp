#include "entity.h"
#include "helper.h"
#include "shaders.h"

#include <GL\glut.h>
#include <iostream>

//	*****************************************  //
//	**            StrangeCube              **  //
//	*****************************************  //

StrangeCube::StrangeCube(World& world):
	Entity(world)
{
	InitShaders();
	InitMesh();
}

StrangeCube::~StrangeCube()
{
	FreeShaders();
	FreeMesh();
}

void StrangeCube::InitShaders()
{
	const char* vsSource = Shaders::simpleVertex;
	const char* frSource = Shaders::simplePhong;

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
	unifTransform = getUniformLocation("transform", program);
	unifCamera = getUniformLocation("camera", program);
	unifProjection = getUniformLocation("projection", program);

	checkOpenGLerror();
}

void StrangeCube::InitMesh()
{
	Point vertexes[] = {
		{-0.5, -0.5, -0.5},	{-0.5, 0.5, -0.5},	{-0.5, 0.5, 0.5},	{-0.5, -0.5, 0.5},
		{0.5, -0.5, -0.5},	{0.5, 0.5, -0.5},	{0.5, 0.5, 0.5},	{0.5, -0.5, 0.5},
		{-0.5, -0.5, -0.5}, {-0.5, 0.5, -0.5},	{0.5, 0.5, -0.5},	{0.5, -0.5, -0.5},
		{-0.5, -0.5, 0.5},	{-0.5, 0.5, 0.5},	{0.5, 0.5, 0.5},	{0.5, -0.5, 0.5},
		{-0.5, -0.5, -0.5},	{0.5, -0.5, -0.5},	{0.5, -0.5, 0.5},	{-0.5, -0.5, 0.5},
		{-0.5, 0.5, -0.5},	{0.5, 0.5, -0.5},	{0.5, 0.5, 0.5},	{-0.5, 0.5, 0.5},
	};

	Point normals[] = {
		{-1.0, 0.0, 0.0},	{-1.0, 0.0, 0.0},	{-1.0, 0.0, 0.0},	{-1.0, 0.0, 0.0},
		{1.0, 0.0, 0.0},	{1.0, 0.0, 0.0},	{1.0, 0.0, 0.0},	{1.0, 0.0, 0.0},
		{0.0, 0.0, -1.0},	{0.0, 0.0, -1.0},	{0.0, 0.0, -1.0},	{0.0, 0.0, -1.0},
		{0.0, 0.0, 1.0},	{0.0, 0.0, 1.0},	{0.0, 0.0, 1.0},	{0.0, 0.0, 1.0},
		{0.0, -1.0, 0.0},	{0.0, -1.0, 0.0},	{0.0, -1.0, 0.0},	{0.0, -1.0, 0.0},
		{0.0, 1.0, 0.0},	{0.0, 1.0, 0.0},	{0.0, 1.0, 0.0},	{0.0, 1.0, 0.0},
	};

	Point colors[] = {
		{0.0, 1.0, 1.0},	{0.0, 0.0, 1.0},	{0.0, 1.0, 0.0},	{1.0, 1.0, 0.0},
		{1.0, 0.0, 1.0},	{1.0, 0.0, 0.0},	{1.0, 1.0, 1.0},	{0.0, 0.0, 0.0},
		{0.0, 1.0, 1.0},	{0.0, 0.0, 1.0},	{1.0, 0.0, 0.0},	{1.0, 0.0, 1.0},
		{1.0, 1.0, 0.0},	{0.0, 1.0, 0.0},	{1.0, 1.0, 1.0},	{0.0, 0.0, 0.0},
		{0.0, 1.0, 1.0},	{1.0, 0.0, 1.0},	{0.0, 0.0, 0.0},	{1.0, 1.0, 0.0},
		{0.0, 0.0, 1.0},	{1.0, 0.0, 0.0},	{1.0, 1.0, 1.0},	{0.0, 1.0, 0.0},
	};

	glGenBuffers(1, &vertexVbo);
	glBindBuffer(GL_ARRAY_BUFFER, vertexVbo);
	glBufferData(GL_ARRAY_BUFFER, sizeof(vertexes), vertexes, GL_STATIC_DRAW);
	glGenBuffers(1, &normalVbo);
	glBindBuffer(GL_ARRAY_BUFFER, normalVbo);
	glBufferData(GL_ARRAY_BUFFER, sizeof(normals), normals, GL_STATIC_DRAW);
	checkOpenGLerror();
}

void StrangeCube::FreeShaders()
{
	glUseProgram(0);
	glDeleteProgram(program);
}

void StrangeCube::FreeMesh()
{
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDeleteBuffers(1, &vertexVbo);
	glDeleteBuffers(1, &normalVbo);
}

void StrangeCube::Draw()
{
	Matrix m = Translation();
	Matrix c = world.Camera();
	Matrix p = world.Projection();

	glUseProgram(program);

	glEnableVertexAttribArray(attribVertex);
	glBindBuffer(GL_ARRAY_BUFFER, vertexVbo);
	glVertexAttribPointer(attribVertex, 3, GL_FLOAT, GL_FALSE, 0, 0);
	glEnableVertexAttribArray(attribVColor);
	glBindBuffer(GL_ARRAY_BUFFER, normalVbo);
	glVertexAttribPointer(attribVColor, 3, GL_FLOAT, GL_FALSE, 0, 0);

	glUniformMatrix4fv(unifTransform, 1, GL_TRUE, m.Data());
	glUniformMatrix4fv(unifCamera, 1, GL_TRUE, c.Data());
	glUniformMatrix4fv(unifProjection, 1, GL_TRUE, p.Data());

	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDrawArrays(GL_QUADS, 0, 24);
	glDisableVertexAttribArray(attribVColor);
	glDisableVertexAttribArray(attribVertex);

	glUseProgram(0);
	checkOpenGLerror();
}



//	*****************************************  //
//	**           TriangleEntity            **  //
//	*****************************************  //

TriangleEntity::TriangleEntity(World& world, const std::string filename) :
	Entity(world), mesh(TriangleMesh(filename).ToMesh()[0])
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
	const char* vsSource = Shaders::simpleVertex;
	const char* frSource = Shaders::simplePhong;

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
	unifTransform = getUniformLocation("transform", program);
	unifCamera = getUniformLocation("camera", program);
	unifProjection = getUniformLocation("projection", program);

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

void TriangleEntity::Draw()
{
	Matrix m = Translation();
	Matrix c = world.Camera();
	Matrix p = world.Projection();

	glUseProgram(program);

	glEnableVertexAttribArray(attribVertex);
	glBindBuffer(GL_ARRAY_BUFFER, vertexVbo);
	glVertexAttribPointer(attribVertex, 3, GL_FLOAT, GL_FALSE, 0, 0);
	glEnableVertexAttribArray(attribVColor);
	glBindBuffer(GL_ARRAY_BUFFER, normalVbo);
	glVertexAttribPointer(attribVColor, 3, GL_FLOAT, GL_FALSE, 0, 0);

	glUniformMatrix4fv(unifTransform, 1, GL_TRUE, m.Data());
	glUniformMatrix4fv(unifCamera, 1, GL_TRUE, c.Data());
	glUniformMatrix4fv(unifProjection, 1, GL_TRUE, p.Data());

	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDrawArrays(GL_TRIANGLES, 0, mesh.normals.size());
	glDisableVertexAttribArray(attribVColor);
	glDisableVertexAttribArray(attribVertex);

	glUseProgram(0);
	checkOpenGLerror();
}


//	*****************************************  //
//	**           TexturedEntity            **  //
//	*****************************************  //

TexturedEntity::TexturedEntity(World& world, const std::string filename) :
	Entity(world), mesh(TriangleMesh(filename).ToMesh()[0])
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
	const char* vsSource = Shaders::simpleVertex;
	const char* frSource = Shaders::simpleTexture;

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

void TexturedEntity::Draw()
{
	Matrix m = Translation();
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

ColorTexturedEntity::ColorTexturedEntity(World& world, const std::string filename) :
	Entity(world), mesh(TriangleMesh(filename).ToMesh()[0])
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
	const char* vsSource = Shaders::colorTextureVertex;
	const char* frSource = Shaders::colorTexture;

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

void ColorTexturedEntity::Draw()
{
	Matrix m = Translation();
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

TexturedTexturedEntity::TexturedTexturedEntity(World& world, const std::string filename) :
	Entity(world), mesh(TriangleMesh(filename).ToMesh()[0])
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
	const char* vsSource = Shaders::simpleVertex;
	const char* frSource = Shaders::textureTexture;

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

void TexturedTexturedEntity::Draw()
{
	Matrix m = Translation();
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

	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDrawArrays(GL_TRIANGLES, 0, mesh.normals.size());
	glDisableVertexAttribArray(attribVColor);
	glDisableVertexAttribArray(attribVertex);

	glUseProgram(0);
	checkOpenGLerror();
}


//	*****************************************  //
//	**         TexturedBlinnEntity         **  //
//	*****************************************  //

TexturedBlinnEntity::TexturedBlinnEntity(World& world, const std::string filename) :
	Entity(world), mesh(TriangleMesh(filename).ToMesh()[0])
{
	InitShaders();
	InitMesh();
}

TexturedBlinnEntity::~TexturedBlinnEntity()
{
	FreeShaders();
	FreeMesh();
}

void TexturedBlinnEntity::InitShaders()
{
	const char* vsSource = Shaders::simpleVertex;
	const char* frSource = Shaders::blinnTexture;

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

	checkOpenGLerror();
}

void TexturedBlinnEntity::InitMesh()
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

void TexturedBlinnEntity::FreeShaders()
{
	glUseProgram(0);
	glDeleteProgram(program);
}

void TexturedBlinnEntity::FreeMesh()
{
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDeleteBuffers(1, &vertexVbo);
	glDeleteBuffers(1, &normalVbo);
	glDeleteBuffers(1, &textureVbo);
}

void TexturedBlinnEntity::Draw()
{
	Matrix m = Translation();
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

	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDrawArrays(GL_TRIANGLES, 0, mesh.normals.size());
	glDisableVertexAttribArray(attribVColor);
	glDisableVertexAttribArray(attribVertex);

	glUseProgram(0);
	checkOpenGLerror();
}


//	*****************************************  //
//	**         TexturedToonEntity         **  //
//	*****************************************  //

TexturedToonEntity::TexturedToonEntity(World& world, const std::string filename) :
	Entity(world), mesh(TriangleMesh(filename).ToMesh()[0])
{
	InitShaders();
	InitMesh();
}

TexturedToonEntity::~TexturedToonEntity()
{
	FreeShaders();
	FreeMesh();
}

void TexturedToonEntity::InitShaders()
{
	const char* vsSource = Shaders::simpleVertex;
	const char* frSource = Shaders::toonTexture;

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

	checkOpenGLerror();
}

void TexturedToonEntity::InitMesh()
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

void TexturedToonEntity::FreeShaders()
{
	glUseProgram(0);
	glDeleteProgram(program);
}

void TexturedToonEntity::FreeMesh()
{
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDeleteBuffers(1, &vertexVbo);
	glDeleteBuffers(1, &normalVbo);
	glDeleteBuffers(1, &textureVbo);
}

void TexturedToonEntity::Draw()
{
	Matrix m = Translation();
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

	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDrawArrays(GL_TRIANGLES, 0, mesh.normals.size());
	glDisableVertexAttribArray(attribVColor);
	glDisableVertexAttribArray(attribVertex);

	glUseProgram(0);
	checkOpenGLerror();
}


//	*****************************************  //
//	**         TexturedOtherEntity         **  //
//	*****************************************  //

TexturedOtherEntity::TexturedOtherEntity(World& world, const std::string filename) :
	Entity(world), mesh(TriangleMesh(filename).ToMesh()[0])
{
	InitShaders();
	InitMesh();
}

TexturedOtherEntity::~TexturedOtherEntity()
{
	FreeShaders();
	FreeMesh();
}

void TexturedOtherEntity::InitShaders()
{
	const char* vsSource = Shaders::simpleVertex;
	const char* frSource = Shaders::otherTexture;

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

	checkOpenGLerror();
}

void TexturedOtherEntity::InitMesh()
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

void TexturedOtherEntity::FreeShaders()
{
	glUseProgram(0);
	glDeleteProgram(program);
}

void TexturedOtherEntity::FreeMesh()
{
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDeleteBuffers(1, &vertexVbo);
	glDeleteBuffers(1, &normalVbo);
	glDeleteBuffers(1, &textureVbo);
}

void TexturedOtherEntity::Draw()
{
	Matrix m = Translation();
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

	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDrawArrays(GL_TRIANGLES, 0, mesh.normals.size());
	glDisableVertexAttribArray(attribVColor);
	glDisableVertexAttribArray(attribVertex);

	glUseProgram(0);
	checkOpenGLerror();
}