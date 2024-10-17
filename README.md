# Transition

## Overview

Transition is an electric-circuit simulator oriented for audio crossover design.

Having designed a circuit on screen. Crossover response SPL, voltage and impedance curves can be inspected.

It is supposed to be a rebirth of the defunct LinearX CrossoverShop.

This is a UWP Windows app. The project is not 100% complete.

## Features

### Current features:
- Full circuit editor
- Animated simulation, live circuit response while tweaking components with a spinner.
- All passive RLC components, and custom Impedance. Component tolerances/precision supported.
- FDNR, Potentiomenter, Transformer and Switch components.
- Active components like OpAmp, H block, Summer and buffer.
- Loudspeaker component, supporting both SPL and Impedance vectorial curves.
- Both grounded and differential voltage simulation.
- Simulation of Voltage across and Current through every passive component directly.
- Simulation of Power of resistors.
- Own Docking Window System.
- Potentiometer taper library.
- OpAmp library.
- Full support for floating sections or totally floating circuit.
- Data import via text files.
- Opening/Saving files in JSON format.
- Ideal, parasitic and exponential models for RLC components.
- Engineering notation for all controls.

### Features Yet to Be Implemented:
- Digital FIR and IIR filters.
- Data Export.
- Graph Export.
- Printing.
- Remove an annoying hint box that shows "Ctrl+F4" message.
- Circuit synthesis.
- Unary math operations (gain, phase offset, etc..).
- Binary math operations.
- Minimum Phase Transform.
- Delay Phase Transform.
- Fast Fourier Transform.

## Getting Started

1. Clone this repository.
2. Clone the ECDockingManager project repository published at [ECDockingManager](https://gitlab.com/alsinaleandro/ecdockingmanager). 
3. Open the Transition solution. Add or fix the ECDockingManager reference.
4. The only dependency for this project is the ECDockingManager. All other dependencies are available via NuGet.
5. Run the project.

## Donate

If you find this project helpful and want to support its ongoing development, consider buying me a coffee or making a donation!
**Bitcoin Address:**
`bc1qmdmkmyvh3tpef6rk7fu5kszqmks7g37tn23gr0`

Thank you for your support!

## License
This project is licensed under the Apache License 2.0

## Contact

If you have any questions or feedback, feel free to reach out to me at:

**Email:** alsinaleandro@yahoo.com.ar
