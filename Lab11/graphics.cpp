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

Cube* Graphics::AddCube(Entity* parent, double size)
{
	Cube* result = new Cube(parent, size);
	GarbageCollector::AddTracking(result);
	return result;
}

Sphere* Graphics::AddSphere(Entity* parent, double radius)
{
	Sphere* result = new Sphere(parent, radius);
	GarbageCollector::AddTracking(result);
	return result;
}

Cone* Graphics::AddCone(Entity* parent, double base, double height)
{
	Cone* result = new Cone(parent, base, height);
	GarbageCollector::AddTracking(result);
	return result;
}

Torus* Graphics::AddTorus(Entity* parent, double innerRadius, double outerRadius)
{
	Torus* result = new Torus(parent, innerRadius, outerRadius);
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

Cube::Cube(Entity* parent, float size) :
	Mesh(parent),
	size(size)
{ }

void Cube::Draw()
{
	glutSolidCube(size);
}


//	*****************************************  //
//	**              Sphere                 **  //
//	*****************************************  //

Sphere::Sphere(Entity* parent, double radius) :
	Mesh(parent),
	radius(radius)
{ }

void Sphere::Draw()
{
	glutSolidSphere(radius, 10, 10);
}


//	*****************************************  //
//	**               Cone                  **  //
//	*****************************************  //

Cone::Cone(Entity* parent, double base, double height) :
	Mesh(parent),
	base(base),
	height(height)
{ }

void Cone::Draw()
{
	glutSolidCone(base, height, 20, 20);
}


//	*****************************************  //
//	**              Torus                  **  //
//	*****************************************  //

Torus::Torus(Entity* parent, double innerRadius, double outerRadius) :
	Mesh(parent),
	innerRadius(innerRadius),
	outerRadius(outerRadius)
{ }

void Torus::Draw()
{
	glutSolidTorus(innerRadius, outerRadius, 10, 30);
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
