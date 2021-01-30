# Report
0_NA_msp1_rapport.docx

# Environnement de développement:
Python:
- Python 3.7
- Pythorch 1.7.1
- Ml agents 0.22.0
 
Unity:
- Unity 2019.3.10f1
- ML Agents 1.6.0 (la version de github pour avoir le grid sensor)
 
# Vidéos
 
Merci de télécharger les vidéos pour profiter de la résolution native.
Elles ne sont pas commentées, c'est pour l'orale.
 
# Jeu

Le .exe est à l'origine du dossier MiniGamesQnn.
Le jeu est compilé uniquement pour les systèmes Windows.

Le jeu supporte les manettes xbox.
 
Contrôle PC :
    Déplacement : les flèches directionnelles
    Saut : Espace
    Tir : V
 
Contrôle Xbox :
    Déplacement : Joystick de gauche
    Saut : A
    Tir : X

Comme il est pas distribué officielllement votre anti virus peut le bloquer ou faire planter le premier démarage (aprés plusieurs secondes), sur toutes les machines testé je n'ai pas eu de faux positif mais ca peut arriver.

Le but du jeu est de ramasser suffisamment de balles vertes pour atteindre un score de 20. Les balles rouges vous font perdre 1 point.
 
Les balles spéciales:
-    Les balles bleues vous accorde un bonus de vitesse pendant quelques secondes
-    Les balles vertes un bouclier
-    Les balles rouges la possibilitée de tirer un projectile
 
 
Il y'a 2 réseau de neurones sur le principe présenté dans le rapport dans la scène :
-    La balle Jaune qui se déplace et evite les joueurs accorde 5 points. (un autre modéle, plus simple avec le processus inversé)
-    Il y'a un autre agent dans la scène qui se déplace et récupère toutes les balles positives, il n'a pas de score, si vous arrivez lui tirer dessus, il est détruit et libère toutes les balles ramassées (bon courage). C'est le model Mr.Vagiator que je réinitialise à chaque fois qu'il passe la premiére étape donc il agît en boucle.
 
Les 3 autres joueurs sont des IA classiques codés comme un genre d'action planner (GOAP).
  
Bon le jeu souffre de bug forcément, j'ai fais en sorte que l'expérience soit confortable.

