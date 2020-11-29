#pragma once

#include <cmath>

struct Point
{
	float x;
	float y;
	float z;
	
	Point operator*(float c) const
	{
		return { x * c, y * c, z * c };
	}

	Point operator/(float c) const
	{
		return { x / c, y / c, z / c };
	}

	Point operator+(const Point& p) const
	{
		return { x + p.x, y + p.y, z + p.z };
	}

	Point operator-(const Point& p) const
	{
		return { x - p.x, y - p.y, z - p.z };
	}

	Point operator-() const
	{
		return { -x, -y, -z };
	}

	float Len()const
	{
		return std::sqrt(x * x + y * y + z * z);
	}

	Point Norm() const
	{
		return *this / Len();
	}

	float Dot(const Point& p)const
	{
		return x * p.x + y * p.y + z * p.z;
	}

	float Sqr()const
	{
		return this->Dot(*this);
	}

	Point Min(const Point& other)
	{
		return Point{
			std::fminf(x, other.x),
			std::fminf(y, other.y),
			std::fminf(z, other.z)
		};
	}

	Point Max(const Point& other)
	{
		return Point{
			std::fmaxf(x, other.x),
			std::fmaxf(y, other.y),
			std::fmaxf(z, other.z)
		};
	}
};