#pragma once
#include "color.h"

struct Material
{
	Color ambient, diffuse, specular, emission;
	signed char shininess;
	void SetActive()
	{
		diffuse.CallGL([=](auto v) {glMaterialfv(GL_FRONT, GL_DIFFUSE, v); });
		ambient.CallGL([=](auto v) {glMaterialfv(GL_FRONT, GL_AMBIENT, v); });
		specular.CallGL([=](auto v) {glMaterialfv(GL_FRONT, GL_SPECULAR, v); });
		emission.CallGL([=](auto v) {glMaterialfv(GL_FRONT, GL_EMISSION, v); });
		glMateriali(GL_FRONT, GL_SHININESS, shininess);
	};
};
