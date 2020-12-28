#include "obj_manager.h"

#include <GL\glew.h>
#include <SOIL2.h>
#include <fstream>
#include <exception>
#include <iostream>

//	*****************************************  //
//	**            TriangleMesh             **  //
//	*****************************************  //

static class TriangleMesh
{
	struct Polygon
	{
		int vertex[3];
		int texture[3];
		int normal[3];
	};

	struct Object
	{
		std::vector<Polygon> polygons;
		std::string mtl;
	};

	struct Obj
	{
		std::unordered_map<std::string, ObjectModel::ObjMaterial> mtlLibrary;

		std::vector<Object> objects;
		std::vector<Point> vertexes;
		std::vector<Point> normales;
		std::vector<Point2> textures;
	};
	
	struct Mesh
	{
		std::vector<Point> points;
		std::vector<Point> normals;
		std::vector<Point2> textures;
	};

	Obj model;
	ObjManager* objManager;

public:
	TriangleMesh(std::string filename, ObjManager* objManager);
	void Fill(ObjectModel* objModel);
};


//	*****************************************  //
//	**             ObjectModel             **  //
//	*****************************************  //


//	*****************************************  //
//	**       ObjectModel::ModelGroup       **  //
//	*****************************************  //

ObjectModel::ModelGroup::ModelGroup(const ObjectModel::ObjMaterial& material, const ObjectModel::ObjMesh& mesh):
	material(material), mesh(mesh)
{ }

//	*****************************************  //
//	**       ObjectModel::ObjMaterial      **  //
//	*****************************************  //

ObjectModel::ObjMaterial::ObjMaterial(Color ambient, Color diffuse, Color specular, signed char shininess, Color emission) :
	ambient(ambient), diffuse(diffuse), specular(specular),
	emission(emission), shininess(shininess)
{ }

ObjectModel::ObjMaterial::ObjMaterial(GLuint diffuseMap) : diffuseMap(diffuseMap)
{ }

GLuint ObjectModel::ObjMaterial::ParseMap(const std::string& filename, ObjManager* objManager)
{
	return objManager->GetTexture(filename);
}


//	*****************************************  //
//	**             ObjManager              **  //
//	*****************************************  //


ObjectModel* ObjManager::GetModel(const std::string& filename)
{
	if (models.find(filename) != models.end())
	{
		return &models[filename];
	}

	TriangleMesh triangleMesh(filename, this);
	triangleMesh.Fill(&models[filename]);
	return &models[filename];
}

GLuint ObjManager::GetTexture(const std::string& filename)
{
	if (textures.find(filename) != textures.end())
	{
		return textures[filename];
	}

	GLuint result = SOIL_load_OGL_texture
	(
		filename.c_str(),
		SOIL_LOAD_AUTO,
		SOIL_CREATE_NEW_ID,
		SOIL_FLAG_MIPMAPS | SOIL_FLAG_INVERT_Y | SOIL_FLAG_NTSC_SAFE_RGB
	);

	if (result == -1)
		throw std::exception((std::string("SOIL loaded ") + filename + SOIL_last_result()).c_str());
	std::cout << ((std::string("SOIL loaded ") + filename + SOIL_last_result()).c_str()) << std::endl;
	textures[filename] = result;
	return result;
}

ObjManager::~ObjManager()
{ }


//	*****************************************  //
//	**		         Legacy                **  //
//	*****************************************  //


static std::vector<std::string> Split(std::string input, const std::string& chars, bool skip_empty = true)
{
	input += chars[0];
	std::vector<std::string> result;
	size_t last = 0;
	for (size_t i = 0; i < input.size(); ++i)
		if (chars.find(input[i]) != std::string::npos)
		{
			if (i == last && skip_empty)
			{
				last = i + 1;
				continue;
			}

			result.push_back(input.substr(last, i - last));
			last = i + 1;
		}
	return result;
}

static std::vector<std::string> Split(std::string input, char c, bool skip_empty = true)
{
	return Split(input, std::string(1, c), skip_empty);
}


//	*****************************************  //
//	**            TriangleMesh             **  //
//	*****************************************  //

static void ParseMaterialTo(std::unordered_map<std::string, ObjectModel::ObjMaterial>& lib, const std::string& filename, ObjManager* objManager)
{
	std::ifstream file(filename);
	std::string s;
	std::string current_name;
	ObjectModel::ObjMaterial* current = nullptr;
	while (!file.eof())
	{
		std::getline(file, s, '\n');
		auto strings = Split(s, " \t");
		if (strings.empty())
			continue;
		if (strings[0] == "newmtl")
		{
			current = &lib[current_name = strings[1]];
		}
		else if (strings[0] == "Ns")
		{
			current->shininess = static_cast<signed char>(std::stof(strings[1]) / 1000 * 128);
		}
		else if (strings[0] == "Ks")
		{
			current->specular = { std::stof(strings[1]), std::stof(strings[2]) , std::stof(strings[3]) };
		}
		else if (strings[0] == "Ka")
		{
			current->ambient = { std::stof(strings[1]), std::stof(strings[2]) , std::stof(strings[3]) };
		}
		else if (strings[0] == "Kd")
		{
			current->diffuse = { std::stof(strings[1]), std::stof(strings[2]) , std::stof(strings[3]) };
		}
		else if (strings[0] == "map_Kd")
		{
			current->diffuseMap = current->ParseMap(strings[1], objManager);
		}
		else if (strings[0] == "map_Ka")
		{
			current->ambientMap = current->ParseMap(strings[1], objManager);
		}
	}
}

void TriangleMesh::Fill(ObjectModel* objModel)
{
	for (size_t i = 0; i < model.objects.size(); ++i)
	{
		Mesh mesh;
		Object& o = model.objects[i];
		mesh.points = std::vector<Point>(o.polygons.size() * 3);
		mesh.normals = std::vector<Point>(o.polygons.size() * 3);
		mesh.textures = std::vector<Point2>(o.polygons.size() * 3);

		for (size_t j = 0; j < o.polygons.size(); ++j)
		{
			Polygon& p = o.polygons[j];

			mesh.points[j * 3] = model.vertexes[p.vertex[0]];
			mesh.points[j * 3 + 1] = model.vertexes[p.vertex[1]];
			mesh.points[j * 3 + 2] = model.vertexes[p.vertex[2]];

			mesh.normals[j * 3] = model.normales[p.normal[0]];
			mesh.normals[j * 3 + 1] = model.normales[p.normal[1]];
			mesh.normals[j * 3 + 2] = model.normales[p.normal[2]];

			mesh.textures[j * 3] = model.textures[p.texture[0]];
			mesh.textures[j * 3 + 1] = model.textures[p.texture[1]];
			mesh.textures[j * 3 + 2] = model.textures[p.texture[2]];
		}

		ObjectModel::ObjMesh result;

		glGenBuffers(1, &result.vertexVbo);
		glBindBuffer(GL_ARRAY_BUFFER, result.vertexVbo);
		glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * mesh.points.size() * 3, mesh.points.data(), GL_STATIC_DRAW);
		glGenBuffers(1, &result.normalVbo);
		glBindBuffer(GL_ARRAY_BUFFER, result.normalVbo);
		glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * mesh.normals.size() * 3, mesh.normals.data(), GL_STATIC_DRAW);
		glGenBuffers(1, &result.textureVbo);
		glBindBuffer(GL_ARRAY_BUFFER, result.textureVbo);
		glBufferData(GL_ARRAY_BUFFER, sizeof(GLfloat) * mesh.textures.size() * 2, mesh.textures.data(), GL_STATIC_DRAW);
		result.size = mesh.points.size();

		objModel->models.push_back(ObjectModel::ModelGroup{ model.mtlLibrary[o.mtl], result });
	}
}


TriangleMesh::TriangleMesh(std::string filename, ObjManager* objManager) :
	objManager(objManager)
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
			ParseMaterialTo(model.mtlLibrary, strings[1], objManager);
		}
		else if (strings[0] == "v")
		{
			Point p{
				std::atof(strings[1].c_str()),
				std::atof(strings[2].c_str()),
				std::atof(strings[3].c_str())
			};
			model.vertexes.push_back(p);
		}
		else if (strings[0] == "vn")
		{
			Point p{
				std::atof(strings[1].c_str()),
				std::atof(strings[2].c_str()),
				std::atof(strings[3].c_str())
			};
			model.normales.push_back(p);
		}
		else if (strings[0] == "vt")
		{
			Point2 p{
				std::atof(strings[1].c_str()),
				std::atof(strings[2].c_str())
			};
			model.textures.push_back(p);
		}
		else if (strings[0] == "g")
		{
			model.objects.emplace_back();
		}
		else if (strings[0] == "usemtl")
		{
			model.objects.back().mtl = strings[1];
		}
		else if (strings[0] == "f")
		{
			for (size_t i = 0; i < strings.size() - 3; ++i)
				model.objects.back().polygons.push_back({
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
