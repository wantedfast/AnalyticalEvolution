import matplotlib.pyplot as plt
import numpy as np

# Plotting the unit circle
theta = np.linspace(0, 2*np.pi, 100)
x = np.cos(theta)
y = np.sin(theta)
plt.plot(x, y, 'b--', label='Unit Circle')

# Plotting the pole
plt.plot(1, 0, 'rx', markersize=10, label='Pole at z=1')

# Adding ROC annotation
plt.annotate('ROC: |z| > 1', xy=(1.5, 0.5))

# Set plot limits
plt.xlim([-2, 2])
plt.ylim([-2, 2])

# Adding labels and legend
plt.xlabel('Real')
plt.ylabel('Imaginary')
plt.title('Z-Plane')
plt.legend()
plt.grid(True)

# Show plot
plt.gca().set_aspect('equal', adjustable='box')
plt.show()
