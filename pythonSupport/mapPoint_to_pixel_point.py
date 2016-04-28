__author__ = 'jiangmb'
import math

def convetToScreentPoint():



    #// (φ)
    latitude    =4970241.327215323;
    # (λ)
    longitude   = 8247861.1000836585;

    mapWidth    = 4096;
    mapHeight   = 4096;

    # get x value
    x = (longitude+180)*(mapWidth/360)
    print(str(x))

    #convert from degrees to radians
    latRad = latitude*math.pi/180

    # get y value
    mercN = math.log(math.tan((math.pi/4)+(latRad/2)));
    y = (mapHeight/2)-(mapWidth*mercN/(2*math.pi));

    print(str(y))
convetToScreentPoint()