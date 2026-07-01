how to create a level:
    enemies:
        enemies follow waypoints. create waypoints by creating empty objects in the hierarchy.
        then assign those waypoint's to the points list in the inspector for the t1enemyMovement component.
        to move waypoints in scene mode, press w

        assign enemy health in the inspector.
        assign enemy a gun by dragging the gun prefab under the enemy in the hierarchy.
        In the "Enemy Attack" script in the inspector, 
            tick "Has Gun" to true,
            set a custom reload time,
            and set a custom stun time (time the guard takes to start shooting after discovering the player).
weapons:
    lunge:
        does 100 damage
        press q
loading in new sprites:
    set its pixels per unit to 32
    set filter mode to point no filter
    set compression to none

level manager:
    drag prefab into scene
    assign it the cinemachine camera in the "Cam" field
    assign it the Fader object in the "Fader" field
    always tag the level manager as level manager (this is used so objects can get its reference)