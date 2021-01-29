# Animation_Foundations_Unique_Delivery_Joan_Queralt
 Unique Delivery Repository for Animation Foundations subject by Joan Queralt

Els scripts utilitzats en aquest projecte són:
- Ball.cs : Script on es controla la trajectoria de la pilota, control de Coroutines per les animacions dels personatges (Robot, Malcolm i Player) i Input 
    del jugador.
- Drawline.cs: Script per dibuixar una primera línia per saber la trajectoria que fa la pilota cap al BlueTarget
- IK_Tentacles.cs: Moviment inicial dels tentacles i crida a la llibreria manipualda.
- LightBlink.cs: Efecte de dispar efecte de brillantor en la pantalla.
- MovingTarget.cs: Control de moviment dels targets limitats a la seva propia regió
- NotifyRegion: Notificats per la col·lisió amb la regió

Apart també s'ha tocat la llibreria Octoupus i s'han tocat els scripts:
- MyOctopusController.cs: Control complert sobre l'octopus i càlcul intern pels 3 diferents Modes que pot tenir amb 
    el seus respectius algorismes: FABRIK, CCD i GRADIENT).
- MyTentacleController.cs: Carrega dels joints del Octopus.


*Inicialment surt un error sobre un prefab missing sobre un "Tree". No afecta al correcte funcionament del projecte.
**Pel correcte funcionament del joc s'ha d'esperar a que acabin totes les animacions que apareixen abans de tornar a xutar.

- Overall, I think that the IK method FABRIK works best for this scenario because the great amount of joints displayed for the Octopus. As is the most
fast and "cheap" alogorithm to calculate has a great performance overall. Gradient and CCD don't show as natural as FABRIK does for the Octopus.