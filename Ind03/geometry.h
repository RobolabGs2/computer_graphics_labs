#pragma once

#include <cmath>
#include <GL\glew.h>

static const float PI = 3.1415926535897932;

struct Point
{
	GLfloat x;
	GLfloat y;
	GLfloat z;

	Point operator*(float c) const
	{
		return { x * c, y * c, z * c };
	}

	Point operator*(const Point& p) const
	{
		return { x * p.x, y * p.y, z * p.z };
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

	Point Abs()const
	{
		return { std::abs(x), std::abs(y), std::abs(z) };
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

struct Point2
{
	GLfloat x;
	GLfloat y;
};

struct Color
{
	GLfloat r;
	GLfloat g;
	GLfloat b;
};

class Matrix
{
	using row = GLfloat[4];
	row data[4];

	Matrix()
	{ }
public:

	static Matrix Ident()
	{
		Matrix m;
		for (int i = 0; i < 4; ++i)
			for (int j = 0; j < 4; ++j)
				m[i][j] = i == j ? 1.0 : 0.0;
		return m;
	}

	static Matrix Zero()
	{
		Matrix m;
		for (int i = 0; i < 4; ++i)
			for (int j = 0; j < 4; ++j)
				m[i][j] = 0;
		return m;
	}

	static Matrix Move(Point p)
	{
		return Move(p.x, p.y, p.z);
	}

	static Matrix Move(float x, float y, float z)
	{
		Matrix m = Ident();
		m[3][0] = x;
		m[3][1] = y;
		m[3][2] = z;
		return m;
	}

	static Matrix Scale(float x, float y, float z)
	{
		Matrix m = Ident();
		m[0][0] = x;
		m[1][1] = y;
		m[2][2] = z;
		return m;
	}

	static Matrix XRotation(float angle)
	{
		return XRotation(sin(angle), cos(angle));
	}

	static Matrix YRotation(float angle)
	{
		return YRotation(sin(angle), cos(angle));
	}

	static Matrix ZRotation(float angle)
	{
		return ZRotation(sin(angle), cos(angle));
	}

	static Matrix XRotation(float sin, float cos)
	{
		Matrix m = Ident();
		m[1][1] = cos;
		m[1][2] = sin;
		m[2][1] = -sin;
		m[2][2] = cos;
		return m;
	}

	static Matrix YRotation(float sin, float cos)
	{
		Matrix m = Ident();
		m[0][0] = cos;
		m[0][2] = -sin;
		m[2][0] = sin;
		m[2][2] = cos;
		return m;
	}

	static Matrix ZRotation(float sin, float cos)
	{
		Matrix m = Ident();
		m[0][0] = cos;
		m[0][1] = sin;
		m[1][0] = -sin;
		m[1][1] = cos;
		return m;
	}

	static Matrix Projection(float rX, float rY, float rZ)
	{
		Matrix m = Ident();
		m[0][3] = rX;
		m[1][3] = rY;
		m[2][3] = rZ;
		return m;
	}

	row& operator[](size_t i)
	{
		return data[i];
	}

	const row& operator[](size_t i) const
	{
		return data[i];
	}

	Matrix operator *(Matrix B)
	{
		Matrix result = Matrix::Zero();
		for (int i = 0; i < 4; ++i)
			for (int j = 0; j < 4; ++j)
				for (int k = 0; k < 4; ++k)
					result[i][j] += (*this)[i][k] * B[k][j];
		return result;
	}


	const float* Data() const
	{
		return (const float*)data;
	}
};

static Point operator *(Point p, Matrix m)
{
	float T = p.x * m[0][3] + p.y * m[1][3] + p.z * m[2][3] + m[3][3];
	return {
		(p.x * m[0][0] + p.y * m[1][0] + p.z * m[2][0] + m[3][0]) / T,
		(p.x * m[0][1] + p.y * m[1][1] + p.z * m[2][1] + m[3][1]) / T,
		(p.x * m[0][2] + p.y * m[1][2] + p.z * m[2][2] + m[3][2]) / T
	};
}

template <typename T>
static int Signum(T val)
{
	if (T(0) < val)
		return -1;
	if (T(0) > val)
		return 1;
	return 0;
}
