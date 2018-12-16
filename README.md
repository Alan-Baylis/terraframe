# Framework TerraFrame
## Uma framework modular para a modelação procedimental de terrenos.

#### A solução é constituída por um projecto que implementa a UI em Unity (pasta TerrainGenerationUI) e um projecto que implementa as bibliotecas do núcleo da framework, estruturas de dados auxiliares e as técnicas de modelação procedimental em C# no VisualStudio (pasta TerrainGeneration).

### Para colocar em funcionamento (Windows):
#### 1- Abrir o projecto TerrainGeneration no VisualStudio e compilar todos os subprojectos para gerar as dll. As dlls compiladas do core e das estruturas auxiliares serão copiadas automáticamente para os assets do projecto do Unity e não requerem nenhuma acção auxiliar.
#### 2- Os dlls dos plugins de modelação procedimental compilados são colocados na pasta TerainGenerationUI/TerrainPlugins. A aplicação procura esses plugins dentro da pasta TerraFrame/TerrainPlugins nos "Meus Documentos" do Windows. Copiar essas dlls e ficheios auxiliares para a pasta {User}/Documents/TerraFrame/TerrainPlugins (criar as pastas se for necessário).
#### 3- Abrir no Unity o projecto contido dentro da pasta TerrainGenerationUI. Iniciar a aplicação em debug dentro do Unity ou exportar. 
