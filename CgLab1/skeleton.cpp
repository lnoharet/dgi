// Introduction lab that covers:
// * C++
// * SDL
// * 2D graphics
// * Plotting pixels
// * Video memory
// * Color representation
// * Linear interpolation
// * glm::vec3 and std::vector

#include "SDL.h"
#include <iostream>
#include <glm/glm.hpp>
#include <vector>
#include "SDLauxiliary.h"

using namespace std;
using glm::vec3;

// --------------------------------------------------------
// GLOBAL VARIABLES

const int SCREEN_WIDTH = 640;
const int SCREEN_HEIGHT = 480;
SDL_Surface* screen;
vector<vec3> leftSide;
vector<vec3> rightSide;

// --------------------------------------------------------
// FUNCTION DECLARATIONS

void Draw();
void Interpolate();
// --------------------------------------------------------
// FUNCTION DEFINITIONS

void Interpolate(float a, float b, vector <float>& result){
	for (int i = 0; i<result.size(); i++){
		float t = (float) i / ((float) result.size() -1);
		result[i] = (1-t) * a + b * t;
	}
}

void Interpolate(vec3 a, vec3 b, vector<vec3>& result){
	for (int i = 0; i<result.size(); i++){
		
		float t = (float) i / ((float) result.size() -1);
		result[i] = vec3 ((1-t) * a.x + b.x * t, (1-t) * a.y + b.y * t, (1-t) * a.z + b.z * t);
	}


}
int main( int argc, char* argv[] )
{

	vec3 topLeft(1,0,0); // red
	vec3 topRight(0,0,1); // blue
	vec3 bottomRight(0,1,0); // green
	vec3 bottomLeft(1,1,0); // yellow

	leftSide = vector<vec3> ( SCREEN_HEIGHT );
	rightSide = vector<vec3> ( SCREEN_HEIGHT );
	Interpolate( topLeft, bottomLeft, leftSide );
	Interpolate( topRight, bottomRight, rightSide );
	screen = InitializeSDL( SCREEN_WIDTH, SCREEN_HEIGHT );
	while( NoQuitMessageSDL() )
	{
		Draw();
	}
	SDL_SaveBMP( screen, "screenshot.bmp" );


	return 0;
	

	//vector<float> result( 10 ); // Create a vector width 10 floats
	//Interpolate( 5, 14, result ); // Fill it with interpolated values
	//for( int i=0; i<result.size(); ++i )
	//	cout << result[i] << " "; // Print the result to the terminal


	/*
	vector<vec3> result( 4 );
	vec3 a(1,4,9.2);
	vec3 b(4,1,9.8);
	Interpolate( a, b, result );
	for( int i=0; i<result.size(); ++i )
	{
		cout << "( "
		<< result[i].x << ", "
		<< result[i].y << ", "
		<< result[i].z << " ) ";
	}*/
	return 0;
}


void Draw()
{

	for( int y=0; y<SCREEN_HEIGHT; ++y )
	{
		vector<vec3> color(SCREEN_WIDTH);
		Interpolate(leftSide[y], rightSide[y], color);



		for( int x=0; x<SCREEN_WIDTH; ++x )
		{
			
			PutPixelSDL( screen, x, y, color[x]);
		}
	}

	if( SDL_MUSTLOCK(screen) )
		SDL_UnlockSurface(screen);

	SDL_UpdateRect( screen, 0, 0, 0, 0 );
}