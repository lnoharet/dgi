///////////////////////////////////////////////////////////////////////////////////////////////////
// OpenGL Mathematics Copyright (c) 2005 - 2012 G-Truc Creation (www.g-truc.net)
///////////////////////////////////////////////////////////////////////////////////////////////////
// Created : 2009-05-07
// Updated : 2009-05-07
// Licence : This source is under MIT License
// File    : glm/gtx/simd_vec4.inl
///////////////////////////////////////////////////////////////////////////////////////////////////

namespace glm{
namespace detail{

template <int Value>
struct mask
{
	enum{value = Value};
};

//////////////////////////////////////
// Implicit basic constructors

GLM_FUNC_QUALIFIER fvec4SIMD::fvec4SIMD()
{}

GLM_FUNC_QUALIFIER fvec4SIMD::fvec4SIMD(__m128 const & Data) :
	Data(Data)
{}

GLM_FUNC_QUALIFIER fvec4SIMD::fvec4SIMD(fvec4SIMD const & v) :
	Data(v.Data)
{}

GLM_FUNC_QUALIFIER fvec4SIMD::fvec4SIMD(tvec4<float> const & v) :
	Data(_mm_set_ps(v.w, v.z, v.y, v.x))
{}

//////////////////////////////////////
// Explicit basic constructors

GLM_FUNC_QUALIFIER fvec4SIMD::fvec4SIMD(float const & s) :
	Data(_mm_set1_ps(s))
{}

GLM_FUNC_QUALIFIER fvec4SIMD::fvec4SIMD(float const & x, float const & y, float const & z, float const & w) :
//		Data(_mm_setr_ps(x, y, z, w))
	Data(_mm_set_ps(w, z, y, x))
{}
/*
GLM_FUNC_QUALIFIER fvec4SIMD::fvec4SIMD(float const v[4]) :
	Data(_mm_load_ps(v))
{}
*/
//////////////////////////////////////
// Swizzle constructors

//fvec4SIMD(ref4<float> const & r);

//////////////////////////////////////
// Convertion vector constructors

GLM_FUNC_QUALIFIER fvec4SIMD::fvec4SIMD(vec2 const & v, float const & s1, float const & s2) :
	Data(_mm_set_ps(s2, s1, v.y, v.x))
{}

GLM_FUNC_QUALIFIER fvec4SIMD::fvec4SIMD(float const & s1, vec2 const & v, float const & s2) :
	Data(_mm_set_ps(s2, v.y, v.x, s1))
{}

GLM_FUNC_QUALIFIER fvec4SIMD::fvec4SIMD(float const & s1, float const & s2, vec2 const & v) :
	Data(_mm_set_ps(v.y, v.x, s2, s1))
{}

GLM_FUNC_QUALIFIER fvec4SIMD::fvec4SIMD(vec3 const & v, float const & s) :
	Data(_mm_set_ps(s, v.z, v.y, v.x))
{}

GLM_FUNC_QUALIFIER fvec4SIMD::fvec4SIMD(float const & s, vec3 const & v) :
	Data(_mm_set_ps(v.z, v.y, v.x, s))
{}

GLM_FUNC_QUALIFIER fvec4SIMD::fvec4SIMD(vec2 const & v1, vec2 const & v2) :
	Data(_mm_set_ps(v2.y, v2.x, v1.y, v1.x))
{}

//GLM_FUNC_QUALIFIER fvec4SIMD::fvec4SIMD(ivec4SIMD const & v) :
//	Data(_mm_cvtepi32_ps(v.Data))
//{}

//////////////////////////////////////
// Unary arithmetic operators

GLM_FUNC_QUALIFIER fvec4SIMD& fvec4SIMD::operator=(fvec4SIMD const & v)
{
	this->Data = v.Data;
	return *this;
}

GLM_FUNC_QUALIFIER fvec4SIMD& fvec4SIMD::operator+=(float const & s)
{
	this->Data = _mm_add_ps(Data, _mm_set_ps1(s));
	return *this;
}

GLM_FUNC_QUALIFIER fvec4SIMD& fvec4SIMD::operator+=(fvec4SIMD const & v)
{
	this->Data = _mm_add_ps(this->Data , v.Data);
	return *this;
}

GLM_FUNC_QUALIFIER fvec4SIMD& fvec4SIMD::operator-=(float const & s)
{
	this->Data = _mm_sub_ps(Data, _mm_set_ps1(s));
	return *this;
}

GLM_FUNC_QUALIFIER fvec4SIMD& fvec4SIMD::operator-=(fvec4SIMD const & v)
{
	this->Data = _mm_sub_ps(this->Data , v.Data);
	return *this;
}

GLM_FUNC_QUALIFIER fvec4SIMD& fvec4SIMD::operator*=(float const & s)
{
	this->Data = _mm_mul_ps(this->Data, _mm_set_ps1(s));
	return *this;
}

GLM_FUNC_QUALIFIER fvec4SIMD& fvec4SIMD::operator*=(fvec4SIMD const & v)
{
	this->Data = _mm_mul_ps(this->Data , v.Data);
	return *this;
}

GLM_FUNC_QUALIFIER fvec4SIMD& fvec4SIMD::operator/=(float const & s)
{
	this->Data = _mm_div_ps(Data, _mm_set1_ps(s));
	return *this;
}

GLM_FUNC_QUALIFIER fvec4SIMD& fvec4SIMD::operator/=(fvec4SIMD const & v)
{
	this->Data = _mm_div_ps(this->Data , v.Data);
	return *this;
}

GLM_FUNC_QUALIFIER fvec4SIMD& fvec4SIMD::operator++()
{
	this->Data = _mm_add_ps(this->Data , glm::detail::one);
	return *this;
}

GLM_FUNC_QUALIFIER fvec4SIMD& fvec4SIMD::operator--()
{
	this->Dat                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   