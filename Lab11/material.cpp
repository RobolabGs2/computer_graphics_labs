#include "material.h"
#include <unordered_map>
#include <SOIL2.h>

std::unordered_map<std::string, GLuint> pictureCache;

void Material::Activate() const
{
	if (diffuseMap == 0)
	{
		diffuse.CallGL([=](auto v) { glMaterialfv(GL_FRONT, GL_DIFFUSE, v); });
		ambient.CallGL([=](auto v) { glMaterialfv(GL_FRONT, GL_AMBIENT, v); });
		specular.CallGL([=](auto v) { glMaterialfv(GL_FRONT, GL_SPECULAR, v); });
		emission.CallGL([=](auto v) { glMaterialfv(GL_FRONT, GL_EMISSION, v); });
		glMateriali(GL_FRONT, GL_SHININESS, shininess);
	}
	else
	{
		glEnable(GL_TEXTURE_2D);
		glBindTexture(GL_TEXTURE_2D, diffuseMap);
		glTexEnvi(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_MODULATE);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	}
}

void Material::Deactivate() const
{
	if (diffuseMap == 0)
	{
		defaultMaterial.Activate();
	}
	else
	{
		glDisable(GL_TEXTURE_2D);
		glBindTexture(GL_TEXTURE_2D, 0);
	}
}

Material::Material(Color ambient, Color diffuse, Color specular, signed char shininess, Color emission) :
	ambient(ambient), diffuse(diffuse), specular(specular),
	emission(emission), shininess(shininess)
{
}

Material::Material(GLuint diffuseMap) : diffuseMap(diffuseMap)
{
}

Material::Material(const std::string& diffuseMapFilename)
{
	if(this->diffuseMap = pictureCache[diffuseMapFilename])
	{
		return;
	}
	this->diffuseMap = pictureCache[diffuseMapFilename] = SOIL_load_OGL_texture
	(
		diffuseMapFilename.c_str(),
		SOIL_LOAD_AUTO,
		SOIL_CREATE_NEW_ID,
		SOIL_FLAG_MIPMAPS | SOIL_FLAG_INVERT_Y | SOIL_FLAG_NTSC_SAFE_RGB
	);
	printf("SOIL loaded %s: '%s'\n", diffuseMapFilename.c_str(), SOIL_last_result());
}

const Material Material::defaultMaterial = Material();
