how to create a level:
    player:
        Place player in scene
        ALWAYS TAG PLAYER AS PLAYER

        if player should have gun, drag gun into scene as a child object
        tick "has gun" in player inspector

    enemies:
        enemies follow waypoints. create waypoints by creating empty objects in the hierarchy.
        then assign those waypoint's to the points list in the inspector for the t1enemyMovement component.
        to move waypoints in scene mode, press w

        ALWAYS TAG ENEMY AS ENEMY

        assign enemy health in the inspector (default = 100).
        assign enemy a gun by dragging the gun prefab under the enemy in the hierarchy.
        In the "Enemy Attack" script in the inspector, 
            tick "Has Gun" to true,
            set a custom reload time (default = 0.5),
            and set a custom stun time (time the guard takes to start shooting after discovering the player) (default = 0.6).
        In the "t1enemyMovement" script in the inspector,
            tick "Drop Weapon On Death" to your choosing.
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
    ALWAYS TAG LEVEL MANAGER AS LEVEL MANAGER (this is used so objects can get its reference)