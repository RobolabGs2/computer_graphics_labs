#include "graphics.h"
#include "common.h"
#include <Windows.h>
#include <gl\freeglut.h>
#include <fstream>
#include <iostream>

//	*****************************************  //
//	**             Graphics                **  //
//	*****************************************  //

void Graphics::Tick(double dt)
{
	GarbageCollector::Tick();
	for (Mesh* m : data)
		m->Tick(dt);
}

Cube* Graphics::AddCube(Entity* parent, double size, const Material& material)
{
	Cube* result = new Cube(parent, material, size);
	GarbageCollector::AddTracking(result);
	return result;
}

Sphere* Graphics::AddSphere(Entity* parent, double radius, const Material& material)
{
	Sphere* result = new Sphere(parent, material, radius);
	GarbageCollector::AddTracking(result);
	return result;
}

Cone* Graphics::AddCone(Entity* parent, double base, double height, const Material& material)
{
	Cone* result = new Cone(parent, material, base, height);
	GarbageCollector::AddTracking(result);
	return result;
}

Torus* Graphics::AddTorus(Entity* parent, double innerRadius, double outerRadius, const Material& material)
{
	Torus* result = new Torus(parent, material, innerRadius, outerRadius);
	GarbageCollector::AddTracking(result);
	return result;
}

Plane* Graphics::AddPlane(Entity* parent, float xSize, float zSize, const Material& material,  int xPartition,
                          int zPartition)
{
	Plane* result = new Plane(parent, xSize, zSize, xPartition, zPartition, material);
	GarbageCollector::AddTracking(result);
	return result;
}

TriangleMesh* Graphics::AddTriangleMesh(Entity* parent, std::string filename)
{
	TriangleMesh* result = new TriangleMesh(parent, filename);
	GarbageCollector::AddTracking(result);
	return result;
}


//	*****************************************  //
//	**               Mesh                  **  //
//	*****************************************  //

Mesh::Mesh(Entity* parent) :
	parent(parent)
{
}

void Mesh::Tick(double dt)
{
	if (!(alive = parent->alive))
		return;
	glPushMatrix();
	parent->TransformGL();
	Draw();
	glPopMatrix();
}


//	*****************************************  //
//	**               Box                   **  //
//	*****************************************  //
Box::Box(Entity* parent, const Material& material, const Point& size): Mesh(parent), size(size), material(material)
{
}

void Box::Draw()
{
	int divide = 10;
	float xk = size.x / divide;
	float yk = size.y / divide;
	float zk = size.z / divide;
	float x0 = -size.x / 2;
	float y0 = -size.y / 2;
	float z0 = -size.z / 2;
	material.Activate();

	glBegin(GL_QUADS);
	{
		// верх
		for (int i = 0; i < divide; ++i)
		{
			for (int j = 0; j < divide; ++j)
			{
				glNormal3f(0, 1, 0);
				glTexCoord2d(0, 0);
				glVertex3f(x0 + i * xk, -y0, z0 + j * zk);
				glNormal3f(0, 1, 0);
				glTexCoord2d(1, 0);
				glVertex3f(x0 + (i + 1) * xk, -y0, z0 + j * zk);
				glNormal3f(0, 1, 0);
				glTexCoord2d(1, 1);
				glVertex3f(x0 + (i + 1) * xk, -y0, z0 + (j + 1) * zk);
				glNormal3f(0, 1, 0);
				glTexCoord2d(0, 1);
				glVertex3f(x0 + i * xk, -y0, z0 + (j + 1) * zk);
			}
		}
		// низ
		for (int i = 0; i < divide; ++i)
		{
			for (int j = 0; j < divide; ++j)
			{
				glNormal3f(0, -1, 0);
				glTexCoord2d(0, 0);
				glVertex3f(x0 + i * xk, y0, z0 + j * zk);
				glNormal3f(0, -1, 0);
				glTexCoord2d(1, 0);
				glVertex3f(x0 + (i + 1) * xk, y0, z0 + j * zk);
				glNormal3f(0, -1, 0);
				glTexCoord2d(1, 1);
				glVertex3f(x0 + (i + 1) * xk, y0, z0 + (j + 1) * zk);
				glNormal3f(0, -1, 0);
				glTexCoord2d(0, 1);
				glVertex3f(x0 + i * xk, 0, y0 + (j + 1) * zk);
			}
		}
		for (int i = 0; i < divide; ++i)
		{
			GLfloat n[3] = { 1, 0, 0 };
			for (int j = 0; j < divide; ++j)
			{
				glNormal3fv(n);
				glTexCoord2d(0, 0);
				glVertex3f(-x0, y0 + i * yk, z0 + j * zk);
				glNormal3fv(n);
				glTexCoord2d(1, 0);
				glVertex3f(-x0, y0 + (i+1) * yk, z0 + j * zk);
				glNormal3fv(n);
				glTexCoord2d(1, 1);
				glVertex3f(-x0, y0 + (i + 1) * yk, z0 + (j+1) * zk);
				glNormal3fv(n);
				glTexCoord2d(0, 1);
				glVertex3f(-x0, y0 + i * yk, z0 + (j + 1) * zk);
			}
		}
		for (int i = 0; i < divide; ++i)
		{
			GLfloat n[3] = { -1, 0, 0 };
			for (int j = 0; j < divide; ++j)
			{
				glNormal3fv(n);
				glTexCoord2d(0, 0);
				glVertex3f(x0, y0 + i * yk, z0 + j * zk);
				glNormal3fv(n);
				glTexCoord2d(1, 0);
				glVertex3f(x0, y0 + (i + 1) * yk, z0 + j * zk);
				glNormal3fv(n);
				glTexCoord2d(1, 1);
				glVertex3f(x0, y0 + (i + 1) * yk, z0 + (j + 1) * zk);
				glNormal3fv(n);
				glTexCoord2d(0, 1);
				glVertex3f(x0, y0 + i * yk, z0 + (j + 1) * zk);
			}
		}
		for (int i = 0; i < divide; ++i)
		{
			GLfloat n[3] = { 0, 0, 1 };
			for (int j = 0; j < divide; ++j)
			{
				glNormal3fv(n);
				glTexCoord2d(0, 0);
				glVertex3f(x0 + j * xk, y0 + i * yk, -z0);
				glNormal3fv(n);
				glTexCoord2d(1, 0);
				glVertex3f(x0 + j * xk, y0 + (i + 1) * yk, -z0);
				glNormal3fv(n);
				glTexCoord2d(1, 1);
				glVertex3f(x0 + (1 + j) * xk, y0 + (i + 1) * yk, -z0);
				glNormal3fv(n);
				glTexCoord2d(0, 1);
				glVertex3f(x0 + (1 + j) * xk, y0 + i * yk, -z0);
			}
		}
		for (int i = 0; i < divide; ++i)
		{
			GLfloat n[3] = { 0, 0, -1 };
			for (int j = 0; j < divide; ++j)
			{
				glNormal3fv(n);
				glTexCoord2d(0, 0);
				glVertex3f(x0 + j * xk, y0 + i * yk, z0);
				glNormal3fv(n);
				glTexCoord2d(1, 0);
				glVertex3f(x0 + j * xk, y0 + (i + 1) * yk, z0);
				glNormal3fv(n);
				glTexCoord2d(1, 1);
				glVertex3f(x0 + (1+j) * xk, y0 + (i + 1) * yk, z0);
				glNormal3fv(n);
				glTexCoord2d(0, 1);
				glVertex3f(x0 + (1 + j) * xk, y0 + i * yk, z0);
			}
		}
	}
	glEnd();
	material.Deactivate();
}

//	*****************************************  //
//	**               Cube                  **  //
//	*****************************************  //

Cube::Cube(Entity* parent, const Material& material, float size) :
	Box(parent, material, { size, size, size }),
	size(size)
{
}


//	*****************************************  //
//	**              Sphere                 **  //
//	*****************************************  //

Sphere::Sphere(Entity* parent, const Material& material, double radius) :
	Mesh(parent),
	radius(radius),
	material(material)
{
}

void Sphere::Draw()
{
	material.Activate();
	auto* quadObj= gluNewQuadric();
	gluQuadricTexture(quadObj, true);
	gluSphere(quadObj, radius, 10, 10);
	gluDeleteQuadric(quadObj);
	material.Deactivate();
}


//	*****************************************  //
//	**               Cone                  **  //
//	*****************************************  //

Cone::Cone(Entity* parent, const Material& material, double base, double height) :
	Mesh(parent),
	base(base),
	height(height),
	material(material)
{
}

void Cone::Draw()
{
	material.Activate();
	glutSolidCone(base, height, 20, 20);
	material.Deactivate();
}


//	*****************************************  //
//	**              Torus                  **  //
//	*****************************************  //

Torus::Torus(Entity* parent, const Material& material, double innerRadius, double outerRadius) :
	Mesh(parent),
	innerRadius(innerRadius),
	outerRadius(outerRadius),
	material(material)
{
}

void Torus::Draw()
{
	material.Activate();
	glutSolidTorus(innerRadius, outerRadius, 10, 30);
	material.Deactivate();
}


//	*****************************************  //
//	**              Plane                  **  //
//	*****************************************  //

Plane::Plane(Entity* parent, float xSize, float zSize, int xPartition, int zPartition, Material material):
	Mesh(parent),
	xSize(xSize),
	zSize(zSize),
	xPartition(xPartition),
	zPartition(zPartition),
	material(material)
{
}



void Plane::Draw()
{
	float xk = xSize / xPartition;
	float zk = zSize / zPartition;
	float x0 = -xSize / 2;
	float z0 = -zSize / 2;
	material.Activate();
	
	glBegin(GL_QUADS);
	{
		for (int i = 0; i < xPartition; ++i)
		{
			for (int j = 0; j < zPartition; ++j)
			{
				glNormal3f(0, 1, 0);
				glTexCoord2d(0, 0);
				glVertex3f(x0 + i * xk, 0, z0 + j * zk);
				glNormal3f(0, 1, 0);
				glTexCoord2d(1, 0);
				glVertex3f(x0 + (i + 1) * xk, 0, z0 + j * zk);
				glNormal3f(0, 1, 0);
				glTexCoord2d(1, 1);
				glVertex3f(x0 + (i + 1) * xk, 0, z0 + (j + 1) * zk);
				glNormal3f(0, 1, 0);
				glTexCoord2d(0, 1);
				glVertex3f(x0 + i * xk, 0, z0 + (j + 1) * zk);
			}
		}
	}
	glEnd();
	material.Deactivate();
}


//	*****************************************  //
//	**           TriangleMesh              **  //
//	*****************************************  //

void ParseMaterialTo(std::unordered_map<std::string, Material>& lib, const std::string& filename)
{
	std::ifstream file(filename);
	std::string s;
	Material* current = nullptr;
	std::cerr << filename << std::endl;
	while (!file.eof())
	{
		std::getline(file, s, '\n');
		auto strings = Split(s, " \t");
		if (strings.empty())
			continue;
		if (strings[0] == "newmtl")
		{
			current = &lib[strings[1]];
			std::cerr << strings[1]<< std::endl;
			current->emission = { 0, 0, 0 };
		}
		else if (strings[0] == "Ns")
		{
			current->shininess = static_cast<signed char>(std::stof(strings[1]) / 1000 * 128);
		}
		else if (strings[0] == "Ks")
		{
			current->specular = {std::stof(strings[1]), std::stof(strings[2]) , std::stof(strings[3]) };
		}
		else if (strings[0] == "Ka")
		{
			current->ambient = { std::stof(strings[1]), std::stof(strings[2]) , std::stof(strings[3]) };
		}
		else if (strings[0] == "Kd")
		{
			current->diffuse = { std::stof(strings[1]), std::stof(strings[2]) , std::stof(strings[3]) };
		}
	}
}

TriangleMesh::TriangleMesh(Entity* parent, std::string filename):
	Mesh(parent)
{
	std::ifstream file(filename);
	std::string s;

	while (!file.eof())
	{
		std::getline(file, s, '\n');
		auto strings = Split(s, ' ');
		if (strings.empty())
			continue;
		if (strings[0] == "mtllib")
		{
			ParseMaterialTo(mtlLibrary, strings[1]);
		}
		else if (strings[0] == "v")
		{
			Point p{
				std::atof(strings[1].c_str()),
				std::atof(strings[2].c_str()),
				std::atof(strings[3].c_str())
			};
			vertexes.push_back(p);
		}
		else if (strings[0] == "vn")
		{
			Point p{
				std::atof(strings[1].c_str()),
				std::atof(strings[2].c_str()),
				std::atof(strings[3].c_str())
			};
			normales.push_back(p);
		}
		else if (strings[0] == "vt")
		{
			Point p{
				std::atof(strings[1].c_str()),
				std::atof(strings[2].c_str()),
				strings.size() < 4 ? 0 : std::atof(strings[3].c_str())
			};
			textures.push_back(p);
		}
		else if (strings[0] == "g")		
		{
			objects.emplace_back();
		}
		else if (strings[0] == "usemtl")
		{
			objects.back().mtl = strings[1];
		}
		else if (strings[0] == "f")
		{
			for (size_t i = 0; i < strings.size() - 3; ++i)
				objects.back().polygons.push_back({
					{
						std::atoi(Split(strings[1], '/')[0].c_str()) - 1,
						std::atoi(Split(strings[2u + i], '/')[0].c_str()) - 1,
						std::atoi(Split(strings[3u + i], '/')[0].c_str()) - 1
					},
					{
						std::atoi(Split(strings[1], '/', false)[1].c_str()) - 1,
						std::atoi(Split(strings[2u + i], '/', false)[1].c_str()) - 1,
						std::atoi(Split(strings[3u + i], '/', false)[1].c_str()) - 1,
					},
					{
						std::atoi(Split(strings[1], '/', false)[2].c_str()) - 1,
						std::atoi(Split(strings[2u + i], '/', false)[2].c_str()) - 1,
						std::atoi(Split(strings[3u + i], '/', false)[2].c_str()) - 1
					}
				});
		}
	}
}

void TriangleMesh::Draw()
{
	glBegin(GL_TRIANGLES);
	for (Object& o : objects) {
		mtlLibrary[o.mtl].Activate();
		for (Polygon& p : o.polygons)
		{
			Point v1 = vertexes[p.vertex[0]];
			Point v2 = vertexes[p.vertex[1]];
			Point v3 = vertexes[p.vertex[2]];
			Point n1 = normales[p.normal[0]];
			Point n2 = normales[p.normal[1]];
			Point n3 = normales[p.normal[2]];
			Point t1 = textures[p.texture[0]];
			Point t2 = textures[p.texture[1]];
			Point t3 = textures[p.texture[2]];
			glNormal3f(n1.x, n1.y, n1.z);
			glTexCoord3f(t1.x, t1.y, t1.z);
			glVertex3f(v1.x, v1.y, v1.z);
			glNormal3f(n2.x, n2.y, n2.z);
			glTexCoord3f(t2.x, t2.y, t2.z);
			glVertex3f(v2.x, v2.y, v2.z);
			glNormal3f(n3.x, n3.y, n3.z);
			glTexCoord3f(t3.x, t3.y, t3.z);
			glVertex3f(v3.x, v3.y, v3.z);
		}
		mtlLibrary[o.mtl].Deactivate();
	}
	glEnd();
}
