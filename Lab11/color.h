#pragma once

#include <Windows.h>
#include <GL/GL.h>

struct Color
{
	float r;
	float g;
	float b;

	Color operator*(float c) const
	{
		return { r * c, g * c, b * c };
	}

	Color operator/(float c) const
	{
		return { r / c, g / c, b / c };
	}

	Color operator+(const Color& p) const
	{
		return { r + p.r, g + p.g, b + p.b };
	}

	Color operator-(const Color& p) const
	{
		return { r - p.r, g - p.g, b - p.b };
	}

	Color operator-() const
	{
		return { -r, -g, -b };
	}

	template<typename Func>
	void CallGL(Func func) const
	{
		GLfloat arr[] = {r, g, b, 1.0};
		func(arr);
	}
};