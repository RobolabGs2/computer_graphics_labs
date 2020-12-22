#include "tools.h"

#include <SOIL2.h>
#include <string>
#include <fstream>
#include <exception>
#include <iostream>

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

void ParseMaterialTo(std::unordered_map<std::string, Material>& lib, const std::string& filename)
{
	std::ifstream file(filename);
	std::string s;
	std::string current_name;
	Material* current = nullptr;
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
			current->diffuseMap = current->ParseMap(strings[1]);
		}
		else if (strings[0] == "map_Ka")
		{
			current->ambientMap = current->ParseMap(strings[1]);
		}
	}
}

std::vector<Mesh> TriangleMesh::ToMesh()
{
	std::vector<Mesh> result(model.objects.size());

	for(size_t i = 0 ; i < model.objects.size(); ++i)
	{
		Object& o = model.objects[i];
		Material& material = model.mtlLibrary[o.mtl];
		result[i].diffuse = material.diffuse;
		result[i].diffuseMap = material.diffuseMap;
		result[i].ambientMap = material.ambientMap;
		result[i].points = std::vector<Point>(o.polygons.size() * 3);
		result[i].normals = std::vector<Point>(o.polygons.size() * 3);
		result[i].textures = std::vector<Point2>(o.polygons.size() * 3);

		for(size_t j = 0; j < o.polygons.size(); ++j)
		{
			Polygon& p = o.polygons[j];

			result[i].points[j * 3] = model.vertexes[p.vertex[0]];
			result[i].points[j * 3 + 1] = model.vertexes[p.vertex[1]];
			result[i].points[j * 3 + 2] = model.vertexes[p.vertex[2]];

			result[i].normals[j * 3] = model.normales[p.normal[0]];
			result[i].normals[j * 3 + 1] = model.normales[p.normal[1]];
			result[i].normals[j * 3 + 2] = model.normales[p.normal[2]];

			result[i].textures[j * 3] = model.textures[p.texture[0]];
			result[i].textures[j * 3 + 1] = model.textures[p.texture[1]];
			result[i].textures[j * 3 + 2] = model.textures[p.texture[2]];
		}
	}
	return result;
}


TriangleMesh::TriangleMesh(std::string filename)
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
			ParseMaterialTo(model.mtlLibrary, strings[1]);
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


Material::Material(Color ambient, Color diffuse, Color specular, signed char shininess, Color emission) :
	ambient(ambient), diffuse(diffuse), specular(specular),
	emission(emission), shininess(shininess)
{ }

Material::Material(GLuint diffuseMap) : diffuseMap(diffuseMap)
{
}

GLuint Material::ParseMap(const std::string& diffuseMapFilename)
{
	GLuint result = SOIL_load_OGL_texture
	(
		diffuseMapFilename.c_str(),
		SOIL_LOAD_AUTO,
		SOIL_CREATE_NEW_ID,
		SOIL_FLAG_MIPMAPS | SOIL_FLAG_INVERT_Y | SOIL_FLAG_NTSC_SAFE_RGB
	);
	if(result == -1)
		throw std::exception((std::string("SOIL loaded ") + diffuseMapFilename + SOIL_last_result()).c_str());
	std::cout << ((std::string("SOIL loaded ") + diffuseMapFilename + SOIL_last_result()).c_str()) <<std::endl;
	return result;
}
