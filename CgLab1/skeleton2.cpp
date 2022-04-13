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

const int SCREEN_WIDTH = 640*2;
const int SCREEN_HEIGHT = 480*2;
SDL_Surface* screen;
vector<vec3> leftSide;
vector<vec3> rightSide;

vector<vec3> stars( 1000 );
const float focal_length = (float) SCREEN_HEIGHT / 2.0;
int ticks;
float velo = 0.0007;
// --------------------------------------------------------
// FUNCTION DECLARATIONS

void Update();
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

void Update()
{
    int t2 = SDL_GetTicks();
    float dt = float(t2-ticks);
    ticks = t2;

    for( int s=0; s<stars.size(); ++s )
    {
        // Add code for update of stars
        if( stars[s].z <= 0 )
            stars[s].z += 1;
        if( stars[s].z > 1 )
            stars[s].z -= 1;
        stars[s].z -= velo * dt;
    }
}

int main( int argc, char* argv[] )
{
    
    for (int i = 0; i<stars.size(); i++){
        
        float r_x = 2* (float(rand()) / float(RAND_MAX))  - 1;
        float r_y = 2* (float(rand()) / float(RAND_MAX))- 1;
        float r_z = float(rand()) / float(RAND_MAX);

        stars[i] = vec3 (r_x, r_y, r_z);

    }

	screen = InitializeSDL( SCREEN_WIDTH, SCREEN_HEIGHT );
    ticks = SDL_GetTicks();
	while( NoQuitMessageSDL() )
	{
        Update();
		Draw();
	}
	SDL_SaveBMP( screen, "screenshot.bmp" );

	return 0;
	}


void Draw()
{

    SDL_FillRect( screen, 0, 0 );
    if( SDL_MUSTLOCK(screen) )
        SDL_LockSurface(screen);
    for( size_t s=0; s<stars.size(); ++s )
    {
        float u = focal_length*stars[s].x /stars[s].z + (float)SCREEN_WIDTH/2.0f ;
        float v = focal_length*stars[s].y /stars[s].z + (float)SCREEN_HEIGHT/2.0f ;
        vec3 color = 0.2f * vec3(1,1,1) / (stars[s].z*stars[s].z);        
        PutPixelSDL( screen, u, v, color);
    }
    if( SDL_MUSTLOCK(screen) )
        SDL_UnlockSurface(screen);
    SDL_UpdateRect( screen, 0, 0, 0, 0 );
}