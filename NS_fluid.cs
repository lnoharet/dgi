using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NS_fluid : MonoBehaviour
{

    public Material smoke;
    private Texture2D smoke_texture;

    static private int h = 64;
    static private int w = 128;

    // obstacles bool matrix
    private bool [,] obstacles = new bool[w,h]; 

    // density
    private float [,] dens = new float[w,h]; 
    private float [,] dens_prev = new float[w,h]; 

    // horizontal velocity 
    private float [,] u = new float[w,h]; 
    private float [,] u_prev = new float[w,h]; 

    // vertical velocity 
    private float [,] v = new float[w,h]; 
    private float [,] v_prev = new float[w,h]; 

    [Header("Simulation Parameters")]
    public float viscosity = 0f;
    public float densdiff = 0.0f;
    public float force = 1.0f;
    public float source = 50.0f;
    public bool static_source = true;
    public bool borders = false; 


    private float velocity_decrease = 1f;
    private Vector3 mouse_pos;
    private Vector2Int deltacoords;
    private Vector2Int coords;
    private Vector2Int prev_coords;

    private int planemask;

    // stepsize
    private float dt = 0;

    // Start is called before the first frame update
    void Start()
    {   
		
        deltacoords = new Vector2Int(-1, -1);
        coords = new Vector2Int(-1, -1);
        prev_coords = new Vector2Int(-1, -1);
		float iii = Math.Max(h*1.0f, w/1.7f);
		this.gameObject.transform.localScale = new Vector3((float)w/iii, 0.0001f,(float)h/iii);
		
        planemask = LayerMask.GetMask("Plane");
        smoke_texture = new Texture2D(w, h);
        GetComponent<Renderer>().material.mainTexture = smoke_texture; 
        smoke_texture.wrapMode = TextureWrapMode.Clamp;
        //smoke_texture.filterMode = FilterMode.Point;

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

    public void set_obstacles(){
        
        /*for (int i = 0; i < w; i++){
            for (int j=0 ; j < h ; j++ ){
                if (i < w/4+5 && i > w/4-5 && j < h/4+5 && i > h/4-5 ){
                    obstacles[i,j] = true;

                }
            }
        }
        */
        
        //Circle
        float circle_size = h/8;

        int center_x = 1*w/4;
        int center_y = 1*h/2;
        for (int i = 0; i < w; i++){
            for (int j=0 ; j < h ; j++ ){
                int dx = i - center_x;
                int dy = j - center_y;
                double distance = Math.Sqrt((dx * dx) + (dy * dy));
                
                //Testing for obstacle creation fix
                double d2 = Math.Sqrt(((dx-1) * (dx-1)) + (dy * dy));
                double d3 = Math.Sqrt(((dx+1) * (dx+1)) + (dy * dy));
                double d4 = Math.Sqrt((dx * dx) + ((dy-1) * (dy-1)));
                double d5 = Math.Sqrt((dx * dx) + ((dy+1) * (dy+1)));

                

                //testing done; remove && and onwards if not a problem
                if(distance < (float) circle_size ){
                    obstacles[i, j] = true;
                }

            }
        }
        
        
        
        
    }

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
        //Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(mousepos);
        
        dt = Time.deltaTime;
        
        for (int i = 0; i < w; i++){
           for (int j=0 ; j < h ; j++ ){
                dens_prev[i,j] = 0f;
                u_prev[i,j] = 0f;
                v_prev[i,j] = 0f;
            }
        }   
        //Set source: TODO
        if(Input.GetMouseButton(0) || Input.GetMouseButton(1)){
            // Get user input as vec3
            Ray ray =  Camera.main.ScreenPointToRay(Input.mousePosition); //

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, planemask))
            {   
                //Debug.Log("Test");
                //Debug.Log(hit.point);

                //Vector2 tempcoords = new Vector2((((hit.point.x + 5)*w / 10)), (((hit.point.z + 5)*h / 10)));
                //Vector2 tempcoords = new Vector2(((hit.point.x)*w), ((hit.point.y)*h));
                
                float iii = Math.Max(h*1.0f, w/1.7f);
                Vector2 tempcoords = new Vector2((hit.point.x/((5f*(float)w)/iii))*((float)w/2)+((float)w/2), (hit.point.y/((5f*(float)h)/iii))*((float)h/2)+((float)h/2));
                //Vector2 tempcoords = new Vector2((hit.point.x+0.5f)*w, (hit.point.y+0.5f)*h);
                
                //float iii = Math.Max(h*1.0f, w/1.8f);
                //Vector2 tempcoords = new Vector2(((hit.point.x+5f)*(float)w/iii)+0.5f, ((hit.point.y+5f)*(float)h/iii)+0.5f);

                coords.x = (int) tempcoords.x;
                coords.y = (int) tempcoords.y;


                if(coords.x > 0 && coords.x < w-1 && coords.y > 0 && coords.y < h-1){
                    //Set density source
                    if(Input.GetMouseButton(0) && !obstacles[coords.x, coords.y]){
                        //TODO: testing
                        dens_prev[coords.x, coords.y] = Math.Max(source - dens[coords.x, coords.y], dens_prev[coords.x, coords.y]);
                    }
                    
                    if (Input.GetMouseButton(0)){
                        if(prev_coords.x != -1 && !obstacles[coords.x, coords.y]){
                            deltacoords = new Vector2Int(coords.x - prev_coords.x, coords.y - prev_coords.y);
        
                            u_prev[coords.x, coords.y] = deltacoords.x*force*100;
                            v_prev[coords.x, coords.y] = deltacoords.y*force*100;
                            Debug.Log(u_prev[coords.x, coords.y]);
                            Debug.Log(v_prev[coords.x, coords.y]);
                        }
                        prev_coords.x = coords.x;
                        prev_coords.y = coords.y;
                    }
                    if(Input.GetMouseButton(1)){
                        int coordsx1;
                        int coordsx2;
                        int coordsy1;
                        int coordsy2;
                        
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
                //Debug.Log(deltacoords.x);
                
            }
        }
        if(!Input.GetMouseButton(0)){
            prev_coords.x = -1;
            prev_coords.y = -1;
        }
        if (static_source){
            //add_static_source(1,h/2 + h/9);
            //add_static_source(1,h/2 - h/9);
            add_static_source(1,h/2 - h/16);
            add_static_source(1,h/2 + h/16);
            add_static_source(1,h/2 + h/6);
            add_static_source(1,h/2 - h/6);

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

    void add_source_dens(float dt){
        int i;
        int j;
        for (i = 1; i < w-1; i++){
            for ( j = 1; j < h-1; j++){
                dens[i,j] += dt * dens_prev[i,j];    
            }
        }
    }

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

    void to_texture(){
        for (int y = 0; y < smoke_texture.height; y++)
        {
            for (int x = 0; x < smoke_texture.width; x++)
            {
                if (obstacles[x,y] == true){
                    smoke_texture.SetPixel(x, y, new Color(0,1,0,1));
                    
                }
                else {
                    //smoke_texture.SetPixel(x, y, new Color((u[x,y]+v[x,y])*dens[x,y],(u[x,y]+v[x,y])*dens[x,y],(u[x,y]+v[x,y])*dens[x,y],1));
                    smoke_texture.SetPixel(x, y, new Color(dens[x,y],dens[x,y],dens[x,y],1));
                }
                
            }
        }
        smoke_texture.Apply();
    }

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

    float[,] advect(int b, float[,] d, float[,] d_prev, float[,] u, float[,] v, float dt){
        float x_prev, y_prev, t0_y, t1_y, t0_x, t1_x;
        for (int x_curr = 1; x_curr < w-1; x_curr++){
            for (int y_curr = 1; y_curr < h-1; y_curr++){
                x_prev = x_curr - dt*(w-2)*u[x_curr,y_curr];
                y_prev = y_curr - dt*(h-2)*v[x_curr,y_curr];
                // check borders
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

    float[,] set_boundaries(int b, float[,] m){
        int i,j;

        for (i = 1; i < w-1; i++){ 
            for (j = 1; j < h-1; j++){ 
                if (obstacles[i,j]){
                    if(b==1){
                        if(!obstacles[i-1,j]){
                            // cell to left is not obstacle
                            m[i,j]   = -m[i-1,j] * velocity_decrease;
                        }
                        else if(!obstacles[i+1,j]){
                            // cell to right is not obstacle
                            m[i,j]   = -m[i+1,j] * velocity_decrease;
                        }
                        else{
                            // middle obstacle cell
                            m[i,j] = 0f;
                        }

                    }  
                    else if(b==2){
                        if(!obstacles[i,j-1]){
                            // cell down is not obstacle
                            m[i,j]   = -m[i,j-1] * velocity_decrease;
                        }
                        else if(!obstacles[i,j+1]){
                            // cell up is not obstacle
                            m[i,j]   = -m[i,j+1] * velocity_decrease;
                        }
                        else{
                            // middle obstacle cell
                            m[i,j] = 0f;
                        }
                           
                    } 
                    else{
                        // set density in obstacle
                        if(!obstacles[i,j-1]){
                            // cell down is not obstacle
                            m[i,j]   = m[i,j-1];
                        }
                        else if(!obstacles[i,j+1]){
                            // cell up is not obstacle
                            m[i,j]   = m[i,j+1];
                        }
                        else if(!obstacles[i-1,j]){
                            // cell to left is not obstacle
                            m[i,j]   = m[i-1,j];
                        }
                        else if(!obstacles[i+1,j]){
                            // cell to right is not obstacle
                            m[i,j]   = m[i+1,j];
                        }
                        else{
                            m[i,j] = 0f;
                        }
                    }
                    // if (b==1 || b==2){
                    //     // Set obtacle corners
                    //     if(!obstacles[i+1,j] && !obstacles[i,j+1] && !obstacles[i+1,j+1]){
                    //         // TOP RIGHT CORNER
                    //         m[i+1,j+1] = (m[i,j+1] + m[i+1,j])/2 ;
                    //     }   
                    //     else if(!obstacles[i+1,j] && !obstacles[i,j-1] && !obstacles[i+1,j-1]){
                    //         // BOTTOM RIGHT CORNER
                    //         m[i+1,j-1] = (m[i,j-1] + m[i+1,j])/2 ;
                    //     }
                    //     else if(!obstacles[i-1,j] && !obstacles[i,j-1] && !obstacles[i-1,j-1]){
                    //         // BOTTOM LEFT CORNER
                    //         m[i-1,j-1] = (m[i,j-1] + m[i-1,j])/2 ;
                    //     }
                    //     else if(!obstacles[i-1,j] && !obstacles[i,j+1] && !obstacles[i-1,j+1]){
                    //         // TOP LEFT CORNER
                    //         m[i-1,j+1] = (m[i,j+1] + m[i-1,j])/2 ;
                    //     }               
                    // }
                }
            }
        }
        if (borders){
            for (i = 1; i < h-1; i++){ 
                if (b==1) { 
                    m[0,i]   = -m[1,i]; 
                    m[w-1,i] = -m[w-2,i]; 
                    
                }
                else { 
                    m[0,i]   = m[1,i]; 
                    m[w-1,i] = m[w-2,i]; 
                }
                
            }
            for (i = 1; i < w-1; i++){
                if (b==2) { 
                    m[i,0]   = -m[i,1];
                    m[i,h-1] = -m[i,h-2]; 
                }
                else { 
                    m[i,0]   = m[i,1];
                    m[i,h-1] = m[i,h-2]; 
                }
            }
            m[0,0]     = 0.5f*(m[1,0]+ m[0,1]);
            m[0,h-1]   = 0.5f*(m[1,h-1]+ m[0,h-2]);
            m[w-1,0]   = 0.5f*(m[w-2,0]+ m[w-1,1]);
            m[w-1,h-1] = 0.5f*(m[w-2,h-1]+ m[w-1,h-2]);
        }
        return m;
    }

    (float[,], float[,], float[,], float[,]) project(float[,] u_curr, float[,] v_curr, float[,] u_prev_, float[,] v_prev_){
        for (int i = 1; i < w - 1; i++){ 
            for (int j = 1; j < h - 1; j++){ 
                v_prev_[i,j] = -0.5f * (1f / (h-2)) *(u_curr[i+1,j] - u_curr[i-1,j] + v_curr[i,j+1] - v_curr[i,j-1] );
                u_prev_[i,j] = 0;
            }
        }
        v_prev_ = set_boundaries(0, v_prev_);
        u_prev_ = set_boundaries(0, u_prev_);
        
        for (int k = 0; k < 20; k++){
            for (int i = 1; i < w - 1; i++){ 
                for (int j = 1; j < h - 1; j++){
                    u_prev_[i,j] = (v_prev_[i,j] + u_prev_[i-1,j] + u_prev_[i+1,j] + u_prev_[i,j-1] + u_prev_[i,j+1])/4f;
                }
            }
            u_prev_ = set_boundaries(0, u_prev_);
        }
        

        for (int i=1; i < w - 1; i++){
            for (int j=1; j < h - 1; j++){
                u_curr[i, j] -= (0.5f* (u_prev_[i+1, j] - u_prev_[i-1, j]) * (w-2)); 
                v_curr[i, j] -= (0.5f* (u_prev_[i, j+1] - u_prev_[i, j-1]) * (h-2)); 
            }
        }

        //Set boundaries
        u_curr = set_boundaries(1, u_curr);
        v_curr = set_boundaries(2, v_curr);

        return (u_curr, v_curr, u_prev_, v_prev_);
    }

    void add_static_source(int source_x, int source_y){
        dens_prev[source_x, source_y] = source;
        u_prev[source_x, source_y] = force*w;
        v_prev[source_x, source_y] = 0f;
    }




}
