how to create a level:
    enemies:
        enemies follow waypoints. create waypoints by creating empty objects in the hierarchy.
        then assign those waypoint's to the points list in the inspector for the t1enemyMovement component.
        to move waypoints in scene mode, press w

        assign enemy health in the inspector.
weapons:
    lunge:
        does 100 damage
        press q
loading in new sprites:
    set its pixels per unit to 32
    set filter mode to point no filter
    set compression to none