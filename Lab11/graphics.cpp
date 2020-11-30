#include "graphics.h"
#include "common.h"
#include <Windows.h>
#include <gl\freeglut.h>
#include <fstream>


//	*****************************************  //
//	**             Graphics                **  //
//	*****************************************  //

void Graphics::Tick(double dt)
{
	GarbageCollector::Tick();
	for (Mesh* m : data)
		m->Tick(dt);
}

Cube* Graphics::AddCube(Entity* parent, const Material& material, double size)
{
	Cube* result = new Cube(parent, material, size);
	GarbageCollector::AddTracking(result);
	return result;
}

Sphere* Graphics::AddSphere(Entity* parent, const Material& material, double radius)
{
	Sphere* result = new Sphere(parent, material, radius);
	GarbageCollector::AddTracking(result);
	return result;
}

Cone* Graphics::AddCone(Entity* parent, const Material& material, double base, double height)
{
	Cone* result = new Cone(parent, material, base, height);
	GarbageCollector::AddTracking(result);
	return result;
}

Torus* Graphics::AddTorus(Entity* parent, const Material& material, double innerRadius, double outerRadius)
{
	Torus* result = new Torus(parent, material, innerRadius, outerRadius);
	GarbageCollector::AddTracking(result);
	return result;
}

Plane* Graphics::AddPlane(Entity* parent, const Material& material, float xSize, float zSize, int xPartition, int zPartition)
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
{ }

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
//	**               Cube                  **  //
//	*****************************************  //

Cube::Cube(Entity* parent, const Material& material, float size) :
	Mesh(parent),
	size(size),
	material(material)
{ }

void Cube::Draw()
{
	material.SetActive();
	glutSolidCube(size);
}


//	*****************************************  //
//	**              Sphere                 **  //
//	*****************************************  //

Sphere::Sphere(Entity* parent, const Material& material, double radius) :
	Mesh(parent),
	radius(radius),
	material(material)
{ }

void Sphere::Draw()
{
	material.SetActive();
	glutSolidSphere(radius, 10, 10);
}


//	*****************************************  //
//	**               Cone                  **  //
//	*****************************************  //

Cone::Cone(Entity* parent, const Material& material, double base, double height) :
	Mesh(parent),
	base(base),
	height(height),
	material(material)
{ }

void Cone::Draw()
{
	material.SetActive();
	glutSolidCone(base, height, 20, 20);
}


//	*****************************************  //
//	**              Torus                  **  //
//	*****************************************  //

Torus::Torus(Entity* parent, const Material& material, double innerRadius, double outerRadius) :
	Mesh(parent),
	innerRadius(innerRadius),
	outerRadius(outerRadius),
	material(material)
{ }

void Torus::Draw()
{
	material.SetActive();
	glutSolidTorus(innerRadius, outerRadius, 10, 30);
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
{ }

void Plane::Draw()
{
	float xk = xSize / xPartition;
	float zk = zSize / zPartition;
	float x0 = -xSize / 2;
	float z0 = -zSize / 2;

	material.SetActive();
	glBegin(GL_QUADS); {
		for (int i = 0; i < xPartition; ++i) {
			for (int j = 0; j < zPartition; ++j)
			{
				glNormal3f(0, 1, 0);
				glVertex3f(x0 + i * xk, 0, z0 + j * zk);
				glNormal3f(0, 1, 0);
				glVertex3f(x0 + (i + 1) * xk, 0, z0 + j * zk);
				glNormal3f(0, 1, 0);
				glVertex3f(x0 + (i + 1) * xk, 0, z0 + (j + 1) * zk);
				glNormal3f(0, 1, 0);
				glVertex3f(x0 + i * xk, 0, z0 + (j + 1) * zk);
			}
		}
	}
	glEnd();
}


//	*****************************************  //
//	**           TriangleMesh              **  //
//	*****************************************  //


TriangleMesh::TriangleMesh(Entity* parent, std::string filename):
	Mesh(parent)
{
	std::ifstream file(filename);
	std::string s;

	while (!file.eof())
	{
		std::getline(file, s, '\n');
		auto strings = Split(s, ' ');
		if (strings.size() == 0)
			continue;
		//std::cout << s << std::endl;
		if (strings[0] == "v")
		{
			Point p{
					std::atof(strings[1].c_str()),
					std::atof(strings[2].c_str()),
					std::atof(strings[3].c_str())
			};
			vertexes.push_back(p);
		}
		if (strings[0] == "vn")
		{
			Point p{
					std::atof(strings[1].c_str()),
					std::atof(strings[2].c_str()),
					std::atof(strings[3].c_str())
			};
			normales.push_back(p);
		}
		else if (strings[0] == "f")
		{
			for (int i = 0; i < strings.size() - 3; ++i)
				polygons.push_back({ {
						std::atoi(Split(strings[1], '/')[0].c_str()) - 1, 
						std::atoi(Split(strings[2 + i], '/')[0].c_str()) - 1, 
						std::atoi(Split(strings[3 + i], '/')[0].c_str()) - 1
					},
					{0, 0, 0}, {
						std::atoi(Split(strings[1], '/', false)[2].c_str()) - 1,
						std::atoi(Split(strings[2 + i], '/', false)[2].c_str()) - 1,
						std::atoi(Split(strings[3 + i], '/', false)[2].c_str()) - 1
					}
				});
		}
	}
}

void TriangleMesh::Draw()
{
	Material m = {
	{0.0314, 0.0314, 0.0353},
	{0.0314, 0.0314, 0.0353},
	{0.3500, 0.3500, 0.3500},
		{0, 0, 0},
		static_cast<signed char>(32.0 / 1000 * 128),
	};
	m.SetActive();
	glBegin(GL_TRIANGLES);
	for (Polygon& p: polygons)
	{
		Point v1 = vertexes[p.vertex[0]];
		Point v2 = vertexes[p.vertex[1]];
		Point v3 = vertexes[p.vertex[2]];
		Point n1 = normales[p.normal[0]];
		Point n2 = normales[p.normal[1]];
		Point n3 = normales[p.normal[2]];
		glNormal3f(n1.x, n1.y, n1.z);
		glVertex3f(v1.x, v1.y, v1.z);
		glNormal3f(n2.x, n2.y, n2.z);
		glVertex3f(v2.x, v2.y, v2.z);
		glNormal3f(n3.x, n3.y, n3.z);
		glVertex3f(v3.x, v3.y, v3.z);
	}
	glEnd();
}
