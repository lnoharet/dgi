using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NS_fluid : MonoBehaviour
{

    public Material smoke;
    private Texture2D smoke_texture;

    static private int h = 128;
    static private int w = 128;

    // Obstacles bool matrix
    private bool [,] obstacles = new bool[w,h]; 

    // Density
    private float [,] dens = new float[w,h]; 
    private float [,] dens_prev = new float[w,h]; 

    // Horizontal velocity 
    private float [,] u = new float[w,h]; 
    private float [,] u_prev = new float[w,h]; 

    // Vertical velocity 
    private float [,] v = new float[w,h]; 
    private float [,] v_prev = new float[w,h]; 

    // Variables
    [Header("Simulation Parameters")]
    public float viscosity = 0f;
    public float densdiff = 0.0f;
    public float force = 1.0f;
    public float source = 50.0f;
    public bool static_source = true;
    public bool borders = false; 

    // Matrix mapping vectors
    private float velocity_decrease = 1f;
    private Vector3 mouse_pos;
    private Vector2Int deltacoords;
    private Vector2Int coords;
    private Vector2Int prev_coords;

    private int planemask;

    // Delta time for steps
    private float dt = 0;

    // Start is called before the first frame update
    void Start()
    {   
		// Create mapping vectors
        deltacoords = new Vector2Int(-1, -1);
        coords = new Vector2Int(-1, -1);
        prev_coords = new Vector2Int(-1, -1);
        
        // Scaling vector for hight and width of window
		float iii = Math.Max(h*1.0f, w/1.7f);
		this.gameObject.transform.localScale = new Vector3((float)w/iii, 0.0001f,(float)h/iii);
		
        // Texture management
        planemask = LayerMask.GetMask("Plane");
        smoke_texture = new Texture2D(w, h);
        GetComponent<Renderer>().material.mainTexture = smoke_texture; 
        smoke_texture.wrapMode = TextureWrapMode.Clamp;

        // Reset the scene
        for (int i = 0; i < w; i++){
            for (int j=0 ; j < h ; j++ ){
                dens[i,j] = 0f;
                u[i,j] = 0f;
                v[i,j] = 0f;
                dens_prev[i,j] = 0f;
                obstacles[i,j] = false;
            }
        }
        if (static_source){
            set_obstacles();
        }
        
        to_texture();
        
    }


    // Creates a circle obstacle in the Preset Scene
    public void set_obstacles(){
        float circle_size = h/8;
        int center_x = 1*w/4;
        int center_y = 1*h/2;
        for (int i = 0; i < w; i++){
            for (int j=0 ; j < h ; j++ ){
                int dx = i - center_x;
                int dy = j - center_y;
                double distance = Math.Sqrt((dx * dx) + (dy * dy));
                
                // Set as obstacle
                if(distance < (float) circle_size ){
                    obstacles[i, j] = true;
                }   
            }   
        }        
    }

    // Reset the scene
    public void clear_scene(){
        for (int i = 0; i < w; i++){
            for (int j=0 ; j < h ; j++ ){
                dens[i,j] = 0f;
                u[i,j] = 0f;
                v[i,j] = 0f;
                dens_prev[i,j] = 0f;
                obstacles[i,j] = false;   
            }
        }
        if (static_source){
            set_obstacles();
        } 
    }

    // Removes boarders from scene.
    public void clear_boarders(){
        for (int j = 0; j < h; j++){ 
                dens[0,j]   = 0f; 
                dens[w-1,j] = 0f; 
                u[0,j] = 0f;
                v[0,j] = 0f;
                u[w-1,j] = 0f;   
                v[w-1,j] = 0f;         
        }
        for (int i = 0; i < w; i++){
                dens[i,0]   = 0f;
                dens[i,h-1] = 0f; 
                u[i,0] = 0f;
                v[i,0] = 0f;
                u[i,h-1] = 0f; 
                v[i,h-1] = 0f;    
        }
    }



    // Update is called once per frame
    void Update()
    {    
        
		if(Input.GetMouseButton(2)){
            clear_scene();
        }
        
        dt = Time.deltaTime;
        
        // Reset the previous density and velocities
        for (int i = 0; i < w; i++){
           for (int j=0 ; j < h ; j++ ){
                dens_prev[i,j] = 0f;
                u_prev[i,j] = 0f;
                v_prev[i,j] = 0f;
            }
        }   

        if(Input.GetMouseButton(0) || Input.GetMouseButton(1)){
            // Get user input through raycasting
            Ray ray =  Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, planemask))
            {                   
                float iii = Math.Max(h*1.0f, w/1.7f);
                Vector2 tempcoords = new Vector2((hit.point.x/((5f*(float)w)/iii))*((float)w/2)+((float)w/2), (hit.point.y/((5f*(float)h)/iii))*((float)h/2)+((float)h/2));

                coords.x = (int) tempcoords.x;
                coords.y = (int) tempcoords.y;

                // If mouse is within the simulation field
                if(coords.x > 0 && coords.x < w-1 && coords.y > 0 && coords.y < h-1){
                    // If left mouse button is being held down; set density source
                    if(Input.GetMouseButton(0) && !obstacles[coords.x, coords.y]){
                        dens_prev[coords.x, coords.y] = Math.Max(source - dens[coords.x, coords.y], dens_prev[coords.x, coords.y]);
                    }
                    
                    // If left mouse button is being held down; add force
                    if (Input.GetMouseButton(0)){
                        if(prev_coords.x != -1 && !obstacles[coords.x, coords.y]){
                            deltacoords = new Vector2Int(coords.x - prev_coords.x, coords.y - prev_coords.y);
        
                            u_prev[coords.x, coords.y] = deltacoords.x*force*100;
                            v_prev[coords.x, coords.y] = deltacoords.y*force*100;
                            
                        }
                        prev_coords.x = coords.x;
                        prev_coords.y = coords.y;
                    }

                    // If right mouse button is being held down; add obstacle
                    if(Input.GetMouseButton(1)){
                        int coordsx1;
                        int coordsx2;
                        int coordsy1;
                        int coordsy2;

                        //Set 4 obstacles; for obstacles to work in a direction, it is required that there is another one adjacent in that direction
                        
                        if (tempcoords.x - coords.x > 0.5){
                            coordsx1 = coords.x;
                            coordsx2 = coords.x+1;
                        } else{
                            coordsx1 = coords.x-1;
                            coordsx2 = coords.x;
                        }
                        if (tempcoords.y - coords.y > 0.5){
                            coordsy1 = coords.y;
                            coordsy2 = coords.y+1;
                        } else{
                            coordsy1 = coords.y-1;
                            coordsy2 = coords.y;
                        }
                        
                        obstacles[coordsx1, coordsy1] = true;
                        obstacles[coordsx1, coordsy2] = true;
                        obstacles[coordsx2, coordsy1] = true;
                        obstacles[coordsx2, coordsy2] = true;
                    }           
                }
            }
        }
        // If left mouse button is NOT held down; set previous to N/A for force to not be applied incorrectly
        if(!Input.GetMouseButton(0)){
            prev_coords.x = -1;
            prev_coords.y = -1;
        }
        // If the static scene is on, add static sources.
        if (static_source){

            add_static_source(2,h/2 - h/16);
            add_static_source(2,h/2 + h/16);
            add_static_source(2,h/2 + h/6);
            add_static_source(2,h/2 - h/6);
            add_static_source(2,h/2 + h/3);
            add_static_source(2,h/2 - h/3);

        }

        // Add source to velocity
        add_source_vel(dt);
        
        // Update previous velocities 
        u_prev = diffuse(u_prev, u, 1, viscosity, 20, dt);
        v_prev = diffuse(v_prev, v, 2, viscosity, 20, dt);
        
        var proj_tup_prev = project(u_prev, v_prev, u, v);
        u_prev = proj_tup_prev.Item1;
        v_prev = proj_tup_prev.Item2;
        u = proj_tup_prev.Item3;
        v = proj_tup_prev.Item4;

        // Update current velocities 
        u = advect(1, u, u_prev, u_prev, v_prev, dt);
        v = advect(2, v, v_prev, u_prev, v_prev, dt);
        
        var proj_tup = project(u, v, u_prev, v_prev);
        u = proj_tup.Item1;
        v = proj_tup.Item2;
        u_prev = proj_tup.Item3;
        v_prev = proj_tup.Item4;

        // Update density
        add_source_dens(dt);
        dens_prev = diffuse(dens_prev, dens, 0, densdiff, 20, dt);
        dens = advect(0, dens, dens_prev, u, v, dt);

        //Set to texture
        to_texture();
               
    }

    // Add density function
    void add_source_dens(float dt){
        int i;
        int j;
        for (i = 1; i < w-1; i++){
            for ( j = 1; j < h-1; j++){
                dens[i,j] += dt * dens_prev[i,j];    
            }
        }
    }

    // Add velocity function
    void add_source_vel(float dt){
        int i;
        int j;
        for (i = 1; i < w-1; i++){
            for ( j = 1; j < h-1; j++){
                v[i,j] += dt * v_prev[i,j];    
                u[i,j] += dt * u_prev[i,j];  
            }
        }
    }

    // Render on the frontend texture
    void to_texture(){
        for (int y = 0; y < smoke_texture.height; y++)
        {
            for (int x = 0; x < smoke_texture.width; x++)
            {
                // If obstacle, make green
                if (obstacles[x,y] == true){
                    smoke_texture.SetPixel(x, y, new Color(0,1,0,1));
                    
                }
                // Otherwise, set to grayscaled density
                else {
                    smoke_texture.SetPixel(x, y, new Color(dens[x,y],dens[x,y],dens[x,y],1));
                }
                
            }
        }
        smoke_texture.Apply();
    }

    // Diffuse function
    float[,] diffuse(float[,] x, float[,] x_prev, int b, float diff, int diff_steps, float dt){
        float a = dt * diff * (w-2) * (h-2);
        for (int k = 0; k < diff_steps; k++){
            for (int i = 1; i < w-1; i++){
                for (int j = 1; j < h-1; j++){
                    x[i,j] = (x_prev[i,j] + a * (x[i-1,j] + x[i+1,j] + x[i,j-1] + x[i,j+1])) / (1f + 4f * a) ;
                }            
            }
            x = set_boundaries(b, x);
        }
        return x;
    }

    // Advect function
    float[,] advect(int b, float[,] d, float[,] d_prev, float[,] u, float[,] v, float dt){
        float x_prev, y_prev, t0_y, t1_y, t0_x, t1_x;
        for (int x_curr = 1; x_curr < w-1; x_curr++){
            for (int y_curr = 1; y_curr < h-1; y_curr++){
                x_prev = x_curr - dt*(w-2)*u[x_curr,y_curr];
                y_prev = y_curr - dt*(h-2)*v[x_curr,y_curr];
                // Check borders
                if (x_prev < 0.5f){
                    x_prev = 0.5f;
                }
                if (y_prev < 0.5f){
                    y_prev = 0.5f;
                }
                if (x_prev > w-1.5f){
                    x_prev = w-1.5f;
                }
                if (y_prev > h-1.5f){
                    y_prev = h-1.5f;
                }
                int x_left = (int) x_prev;
                int x_right = (int) x_left + 1;
                int y_left = (int) y_prev;
                int y_right = (int) y_left + 1;

                t1_x = x_prev - x_left;
                t0_x = 1f - t1_x;
                t1_y = y_prev - y_left;
                t0_y = 1f - t1_y;
                d[x_curr, y_curr] = t0_x*( t0_y * d_prev[x_left, y_left] + t1_y* d_prev[x_left, y_right]) + 
                                    t1_x*( t0_y * d_prev[x_right, y_left] + t1_y* d_prev[x_right, y_right] );
            }

        }
        d = set_boundaries(b, d);
        return d;
    }

    // Set boundaries
    float[,] set_boundaries(int b, float[,] m){
        int i,j;
        for (i = 1; i < w-1; i++){ 
            for (j = 1; j < h-1; j++){ 
                if (obstacles[i,j]){
                    if(b==1){
                        if(!obstacles[i-1,j]){
                            // Cell to left is not obstacle
                            m[i,j]   = -m[i-1,j] * velocity_decrease;
                        }
                        else if(!obstacles[i+1,j]){
                            // Cell to right is not obstacle
                            m[i,j]   = -m[i+1,j] * velocity_decrease;
                        }
                        else{
                            // Middle obstacle cell
                            m[i,j] = 0f;
                        }
                    }  
                    else if(b==2){
                        if(!obstacles[i,j-1]){
                            // Cell down is not obstacle
                            m[i,j]   = -m[i,j-1] * velocity_decrease;
                        }
                        else if(!obstacles[i,j+1]){
                            // Cell up is not obstacle
                            m[i,j]   = -m[i,j+1] * velocity_decrease;
                        }
                        else{
                            // Middle obstacle cell
                            m[i,j] = 0f;
                        }
                    } 
                    else{
                        // Set density in obstacle
                        if(!obstacles[i,j-1]){
                            // Cell down is not obstacle
                            m[i,j]   = m[i,j-1];
                        }
                        else if(!obstacles[i,j+1]){
                            // Cell up is not obstacle
                            m[i,j]   = m[i,j+1];
                        }
                        else if(!obstacles[i-1,j]){
                            // Cell to left is not obstacle
                            m[i,j]   = m[i-1,j];
                        }
                        else if(!obstacles[i+1,j]){
                            // Cell to right is not obstacle
                            m[i,j]   = m[i+1,j];
                        }
                        else{
                            m[i,j] = 0f;
                        }
                    }
                }
            }
        }
        if (borders){
            for (i = 1; i < h-1; i++){ 
                // Update left and right border
                if (b==1) { // Velocity 
                    m[0,i]   = -m[1,i]; 
                    m[w-1,i] = -m[w-2,i];                     
                }
                else { // Density
                    m[0,i]   = m[1,i]; 
                    m[w-1,i] = m[w-2,i]; 
                }                
            }
            for (i = 1; i < w-1; i++){
                // Update top and bottom border
                if (b==2) { // Velocity
                    m[i,0]   = -m[i,1];
                    m[i,h-1] = -m[i,h-2]; 
                }
                else { // Denisty
                    m[i,0]   = m[i,1];
                    m[i,h-1] = m[i,h-2]; 
                }
            }
            // Update border corners
            m[0,0]     = 0.5f*(m[1,0]+ m[0,1]);
            m[0,h-1]   = 0.5f*(m[1,h-1]+ m[0,h-2]);
            m[w-1,0]   = 0.5f*(m[w-2,0]+ m[w-1,1]);
            m[w-1,h-1] = 0.5f*(m[w-2,h-1]+ m[w-1,h-2]);
        }
        return m;
    }

    // Project function
    (float[,], float[,], float[,], float[,]) project(float[,] u_curr, float[,] v_curr, float[,] u_prev_, float[,] v_prev_){
        for (int i = 1; i < w - 1; i++){ 
            for (int j = 1; j < h - 1; j++){ 
                //
                v_prev_[i,j] = -0.5f * (1f / (h-2)) *(u_curr[i+1,j] - u_curr[i-1,j] + v_curr[i,j+1] - v_curr[i,j-1] );
                u_prev_[i,j] = 0;
            }
        }
        // Check boundaries
        v_prev_ = set_boundaries(0, v_prev_);
        u_prev_ = set_boundaries(0, u_prev_);
        
        // Calculate velocity from surroundings
        for (int k = 0; k < 20; k++){
            for (int i = 1; i < w - 1; i++){ 
                for (int j = 1; j < h - 1; j++){
                    u_prev_[i,j] = (v_prev_[i,j] + u_prev_[i-1,j] + u_prev_[i+1,j] + u_prev_[i,j-1] + u_prev_[i,j+1])/4f;
                }
            }
            // Check boundaries
            u_prev_ = set_boundaries(0, u_prev_);
        }
        // Average velocities
        for (int i=1; i < w - 1; i++){
            for (int j=1; j < h - 1; j++){
                u_curr[i, j] -= (0.5f* (u_prev_[i+1, j] - u_prev_[i-1, j]) * (w-2)); 
                v_curr[i, j] -= (0.5f* (u_prev_[i, j+1] - u_prev_[i, j-1]) * (h-2)); 
            }
        }

        // Check boundaries
        u_curr = set_boundaries(1, u_curr);
        v_curr = set_boundaries(2, v_curr);

        return (u_curr, v_curr, u_prev_, v_prev_);
    }

    // Function for static addition from source
    void add_static_source(int source_x, int source_y){
        dens_prev[source_x, source_y] = source;
        u_prev[source_x, source_y] = force*w;
        v_prev[source_x, source_y] = 0f;
    }

}
