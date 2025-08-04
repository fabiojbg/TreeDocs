import React from 'react';
import { render, screen } from '@testing-library/react';
import Header from '@/components/layout/Header';

describe('Header', () => {
  test('renders Header component', () => {
    render(<Header />);
    screen.debug(); // This will print the rendered component's HTML to the console
    expect(screen.getByText(/TreeNotes/i)).toBeInTheDocument();
  });
});
