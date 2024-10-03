# Plugin TCC - Linguaguem C#
## Criação de um plug-in dentro do software Revit Autodesk, que faz a verificação para paredes de alvenaria estrutural solicitadas a flexocompressão.

Considerações:

<img src="https://latex.codecogs.com/svg.latex?Prd\;{\geq}\;Pd\"/>
<img src="https://latex.codecogs.com/svg.latex?Mrd\;{\geq}\;Md\"/>

<img src="https://latex.codecogs.com/svg.latex?Prd=\;C-\sum\limits_{}^{}ti"/>
<img src="https://latex.codecogs.com/svg.latex?Mrd=\;C*(\frac{l}{2}-\frac{0.8x}{2})+\sum\limits_{}^{}[Ti*(di-\frac{l}{2})"/>

<img src="https://latex.codecogs.com/svg.latex?C=\;0,8*b*x*k*Fd"/>



1- Selecionar a parede no Revit que será verificada.


![image](https://user-images.githubusercontent.com/51210094/151546030-f0089463-d6ed-4df5-9678-14b91e08e82b.png)


2- Ajustar os valores de entrada.

![image](https://user-images.githubusercontent.com/51210094/151546296-42213f98-4159-4606-a699-af4c0c190fe1.png)


3 - Interpretar gráfico de resistência da parede e fazer as devidas analises.

![image](https://user-images.githubusercontent.com/51210094/151545992-1bba0863-484b-4556-8872-669946d2c66f.png)

